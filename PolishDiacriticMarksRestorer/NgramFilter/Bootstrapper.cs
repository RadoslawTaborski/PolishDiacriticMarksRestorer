using NgramFilter.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;

namespace NgramFilter
{
    /// <summary>
    /// Bootstrapper class runs filter, modifier, create output file and database creator.
    /// </summary>
    internal class Bootstrapper
    {
        #region FIELDS
        private readonly IFilter _filter;
        private readonly IModifier _modifier;
        private readonly IFileSystem _fileSystem;
        private readonly IDataBaseManagerFactory _dataAccess;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="modifier">The modifier.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="dataAccess">The data access.</param>
        public Bootstrapper(IFilter filter, IModifier modifier, IFileSystem fileSystem, IDataBaseManagerFactory dataAccess)
        {
            _filter = filter;
            _modifier = modifier;
            _fileSystem = fileSystem;
            _dataAccess = dataAccess;
        }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method runs Filter and create output file.
        /// </summary>
        /// <param name="input">Path to input file.</param>
        /// <param name="output">Path to output file.</param>
        public void Filter(string input, string output)
        {
            using (IFileAccess inputManager = new FileManager(_fileSystem, input))
            using (IFileAccess outputManager = new FileManager(_fileSystem, output))
            {
                outputManager.Create();
                var numberOfLines = inputManager.CountLines();
                inputManager.Open(FileManagerType.Read);
                outputManager.Open(FileManagerType.Write);
                var counter = 0;

                string str;
                while ((str = inputManager.ReadLine()) != null)
                {
                    var list = str.Split(' ').ToList().Where(s => s != "").ToList();
                    var ngram = new NGram
                    {
                        Value = int.Parse(list[0]),
                        WordsList = list.GetRange(1, list.Count - 1)
                    };
                    ngram = _modifier.Start(ngram);
                    var filterResult = _filter.Start(ngram);
                    ++counter;
                    var percent = (double)counter * 100 / numberOfLines;
                    Console.Write(percent.ToString("F3", CultureInfo.InvariantCulture) + "%\r");
                    if (!filterResult) continue;

                    outputManager.WriteLine(ngram.ToString());
                }

                Console.WriteLine("Ukończono pomyślnie\n");
            }
        }

        /// <summary>
        /// This method create database from data from file.
        /// </summary>
        /// <param name="input">Path to input file.</param>
        /// <param name="dbName">Name of database.</param>
        /// <param name="tableName">Name of table.</param>
        public void CreateDb(string input, string dbName, string tableName)
        {
            var first = true;
            using (var inputManager = new FileManager(_fileSystem, input))
            using (var dbManager =
                new DataBaseManager(_dataAccess, "localhost", dbName, "root", ""))
            {
                var numberOfLines = inputManager.CountLines();
                var counter = 0;
                inputManager.Open(FileManagerType.Read);
                IDataBaseCreator creator = new NgramsDataBaseCreator2(dbManager);

                string str;
                var ngrams = new List<NGram>();
                while ((str = inputManager.ReadLine()) != null)
                {
                    var list = str.Split(' ').ToList().Where(s => s != "").ToList();
                    var ngram = new NGram
                    {
                        Value = int.Parse(list[0]),
                        WordsList = list.GetRange(1, list.Count - 1)
                    };
                    if (first)
                    {
                        creator.CreateDataBase(dbName);
                        creator.CreateTables(dbName, tableName, ngram.WordsList.Count);
                        first = false;
                    }
                    ngram.ChangeSpecialCharacters();
                    ngrams.Add(ngram);
                    if (counter % 800 == 0)
                    {
                        if (_modifier.Size() == 0)
                            creator.AddNgramsToTable(tableName, ngrams);
                        else
                            creator.AddOrUpdateNgramsToTable(tableName, ngrams);
                        ngrams = new List<NGram>();
                    }

                    ++counter;
                    var percent = (double)counter * 100 / numberOfLines;
                    Console.Write(percent.ToString("F3", CultureInfo.InvariantCulture) + "%\r");
                }

                if (_modifier.Size() == 0)
                    creator.AddNgramsToTable(tableName, ngrams);
                else
                    creator.AddOrUpdateNgramsToTable(tableName, ngrams);

                Console.WriteLine("Ukończono pomyślnie");
            }
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}

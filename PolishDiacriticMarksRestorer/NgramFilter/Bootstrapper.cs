using NgramFilter.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using MySql.Data.MySqlClient;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;

namespace NgramFilter
{
    internal class Bootstrapper
    {
        private readonly IFilter _filter;
        private readonly IFileSystem _fileSystem;
        private readonly IDataBaseManagerFactory _dataAccess;

        public Bootstrapper(IFilter filter, IFileSystem fileSystem, IDataBaseManagerFactory dataAccess)
        {
            _filter = filter;
            _fileSystem = fileSystem;
            _dataAccess = dataAccess;
        }

        public void Filter(string input, string output)
        {
            try
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
            catch (IOException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
        }

        public void CreateDb(string input, string dbName, string tableName, int numberOfWords)
        {
            try
            {
                using (var inputManager = new FileManager(_fileSystem, input))
                using (var dbManager =
                    new DataBaseManager(_dataAccess, "localhost", "NGrams", "root", ""))
                {
                    var numberOfLines = inputManager.CountLines();
                    var counter = 0;
                    inputManager.Open(FileManagerType.Read);
                    {
                        IDataBaseCreator creator = new NgramsDataBaseCreator(dbManager);
                        creator.CreateDataBase(dbName);
                        creator.CreateTable(dbName, tableName, numberOfWords);

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
                            ngram.ChangeSpecialCharacters();
                            ngrams.Add(ngram);
                            if (counter % 800 == 0)
                            {
                                creator.AddNgramsToTable(tableName, ngrams);
                                ngrams = new List<NGram>();
                            }

                            ++counter;
                            var percent = (double)counter * 100 / numberOfLines;
                            Console.Write(percent.ToString("F3", CultureInfo.InvariantCulture) + "%\r");
                        }

                        creator.AddNgramsToTable(tableName, ngrams);

                        Console.WriteLine("Ukończono pomyślnie");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
        }
    }
}

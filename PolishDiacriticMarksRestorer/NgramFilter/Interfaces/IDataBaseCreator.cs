using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    internal interface IDataBaseCreator
    {
        /// <summary>
        /// This method create new database on server
        /// </summary>
        /// <param name="name">name of database</param>
        void CreateDataBase(string name);
        /// <summary>
        /// This method create new table in database
        /// </summary>
        /// <param name="dataBaseName">name of database</param>
        /// <param name="tableName">name of table</param>
        /// <param name="numberOfWords">number of column with words</param>
        void CreateTable(string dataBaseName, string tableName, int numberOfWords);
        /// <summary>
        /// This method add Ngrams data to table
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="ngrams">list of ngrams</param>
        void AddNgramsToTable(string tableName, List<NGram> ngrams);
        /// <summary>
        /// This method add news ngrams or update if already exists the same ngram in database
        /// </summary>
        /// <param name="tableName">name of table</param>
        /// <param name="ngrams">list of ngrams</param>
        void AddOrUpdateNgramsToTable(string tableName, List<NGram> ngrams);
    }
}

using System;
using System.Collections.Generic;
using NgramAnalyzer.Common;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer
{
    /// <summary>
    /// SqlQueryProvider Class provides MySql Query
    /// </summary>
    public class SqlQueryProvider : IQueryProvider
    {
        #region FIELDS
        private readonly IList<string> _dbTableDbTableName;
        #endregion

        #region CONSTRUCTORS
        public SqlQueryProvider(IList<string> dbTableNames)
        {
            if (dbTableNames == null || dbTableNames.Count != 4)
                throw new ArgumentException("IList<string> 'dbTableNames' has wrong size");
            _dbTableDbTableName = dbTableNames;
        }
        #endregion

        #region  PUBLIC
        /// <inheritdoc />
        public string GetNgramsFromTable(NgramType ngramType, List<string> wordList)
        {
            var number = (int)ngramType;

            if (wordList == null || wordList.Count < number)
                throw new ArgumentException("List<string> 'wordList' has wrong size");

            var query = "SELECT * FROM " + _dbTableDbTableName[number - 1] + " WHERE ";

            for (var i = 0; i < number; ++i)
            {
                if (i != 0) query += "AND ";
                query += "Word" + (i + 1) + "='" + wordList[i] + "' ";
            }

            var strBuilder =
                new System.Text.StringBuilder(query) { [query.Length - 1] = ';' };
            query = strBuilder.ToString();

            return query;
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using NgramAnalyzer.Common;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer
{
    /// <summary>
    /// SqlQueryProvider Class provides MySql Query.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IQueryProvider" />
    public class SqlQueryProvider : IQueryProvider
    {
        #region FIELDS
        private readonly IList<string> _dbTableDbTableName;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryProvider"/> class.
        /// </summary>
        /// <param name="dbTableNames">The database table names.</param>
        /// <exception cref="ArgumentException">IList (string) 'dbTableNames' has wrong size</exception>
        public SqlQueryProvider(IList<string> dbTableNames)
        {
            if (dbTableNames == null || dbTableNames.Count != 4)
                throw new ArgumentException("IList<string> 'dbTableNames' has wrong size");
            _dbTableDbTableName = dbTableNames;
        }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// Generate query to get data from Ngrams table.
        /// </summary>
        /// <param name="ngramType">Type of NGrams.</param>
        /// <param name="wordList">List of words - must have a size suitable for the type ngrams.</param>
        /// <returns>
        /// Query string.
        /// </returns>
        /// <exception cref="ArgumentException">List(string) 'wordList' has wrong size</exception>
        /// <inheritdoc />
        public string GetTheSameNgramsFromTable(NgramType ngramType, List<string> wordList)
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

        /// <inheritdoc />
        /// <summary>
        /// Generate query to get the similar ngrams from Ngrams table.
        /// </summary>
        /// <param name="ngramType">Type of the ngram.</param>
        /// <param name="wordList">List of words - must have a one size smaller than the type ngrams.</param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentException">
        /// List(string) 'wordList' has wrong size
        /// or
        /// NgramType 'ngramType' cannot be an Unigram
        /// </exception>
        public string GetSimilarNgramsFromTable(NgramType ngramType, List<string> wordList)
        {
            var number = (int)ngramType;
            var numberComparedWords = number - 1;

            if (wordList == null || wordList.Count < numberComparedWords)
                throw new ArgumentException("List<string> 'wordList' has wrong size");
            if (ngramType == NgramType.Unigram)
                throw new ArgumentException("NgramType 'ngramType' cannot be an Unigram");

            var query = QueryCreator(number, numberComparedWords, wordList);

            return query;
        }
        #endregion

        #region PRIVATE

        private string QueryCreator(int ngramSize, int numberComparedWords, IReadOnlyList<string> wordList)
        {
            var query = "SELECT * FROM " + _dbTableDbTableName[ngramSize - 1] + " WHERE ";

            for (var i = 0; i < numberComparedWords; ++i)
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
    }
}

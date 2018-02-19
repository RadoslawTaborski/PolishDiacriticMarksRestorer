using System;
using System.Collections.Generic;
using System.Linq;
using NgramAnalyzer.Common;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer
{
    /// <summary>
    /// SqlQueryProvider2 Class provides MySql Query.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IQueryProvider" />
    public class SqlQueryProvider2 : IQueryProvider
    {
        #region FIELDS
        private readonly IList<string> _dbTableDbTableName;
        public static readonly string[] Names = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryProvider"/> class.
        /// </summary>
        /// <param name="dbTableNames">The database table names.</param>
        /// <exception cref="ArgumentException">IList (string) 'dbTableNames' has wrong size</exception>
        public SqlQueryProvider2(IList<string> dbTableNames)
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

            var query = "SELECT * FROM " + _dbTableDbTableName[number - 1] + " WHERE";

            for (var i = 0; i < number; ++i)
            {
                if (i != 0) query += " AND";
                query += " Word" + (i + 1) + "='" + wordList[i].ChangeSpecialCharacters() + "'";
            }

            query += ";";

            return query;
        }

        /// <summary>
        /// Generate query which gets the ngrams changing the last word from table.
        /// </summary>
        /// <param name="ngramType">Type of the ngram.</param>
        /// <param name="wordList">The word list.</param>
        /// <param name="combinations">Possible last words.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// List(string) 'wordList' has wrong size
        /// or
        /// NgramType 'ngramType' cannot be an Unigram
        /// </exception>
        public string GetMultiNgramsFromTable(NgramType ngramType, List<string> wordList, List<string> combinations)
        {
            var number = (int)ngramType;

            if (wordList == null || wordList.Count != number - 1)
                throw new ArgumentException("List<string> 'wordList' has wrong size");
            if (combinations == null || combinations.Count < 1)
                throw new ArgumentException("List<string> 'combinations' has wrong size");

            var query = "SELECT * FROM " + _dbTableDbTableName[number - 1] + " WHERE ";

            for (var i = 0; i < number - 1; ++i)
            {
                if (i != 0) query += "AND ";
                query += "Word" + (i + 1) + "='" + wordList[i].ChangeSpecialCharacters() + "' ";
            }
            query += "AND ( ";
            for (var i = 0; i < combinations.Count; ++i)
            {
                if (i != 0) query += "OR ";
                query += "Word" + number + "='" + combinations[i].ChangeSpecialCharacters() + "' ";
            }
            query += ");";

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

            var query = QueryCreator(number, numberComparedWords, wordList);

            return query;
        }

        public string CheckWordsInUnigramFromTable(List<string> wordList)
        {
            if (wordList == null || wordList.Count == 0)
                throw new ArgumentException("List<string> 'wordList' can't be null");

            var comandsText = new string[Names.Length];
            for (var index = 0; index < comandsText.Length; index++)
            {
                comandsText[index] = "";
            }

            foreach (var item in wordList)
            {
                var index = GetIndexOfNames(item);
                if (index == -1) continue;
                if (comandsText[index] == "")
                {
                    comandsText[index] = "SELECT * FROM `" + _dbTableDbTableName[0] + "[" + Names[index] + "]` WHERE";
                    comandsText[index] += " Word1='" + item.ChangeSpecialCharacters() + "'";
                }
                else
                {
                    comandsText[index] += " OR";
                    comandsText[index] += " Word1='" + item.ChangeSpecialCharacters() + "'";
                }
            }

            var query = comandsText.Where(item => item != "").Aggregate("", (current, item) => current + (item + " UNION ALL "));
            query = query.Substring(0, query.Length - 11);
            query += ";";
            return query;
        }

        public string GetAllNecessaryNgramsFromTable(NgramType ngramType, List<List<List<string>>> wordLists)
        {
            var number = (int)ngramType;

            if (wordLists == null || wordLists.Count < 1)
                throw new ArgumentException("List<string> 'wordLists' has wrong size");
            foreach (var item in wordLists)
            {
                if (item.Count != number)
                    throw new ArgumentException("List<string> middle list has wrong size");
                foreach (var item2 in item)
                {
                    if (item2.Count < 1)
                        throw new ArgumentException("List<string> inside list has wrong size");
                }
            }


            var query = "SELECT * FROM " + _dbTableDbTableName[number - 1] + " WHERE ";

            var z = 1;
            foreach (var item1 in wordLists)
            {
                var j = 1;

                if (z != 1) query += " OR ";

                query += "( ";
                foreach (var item2 in item1)
                {
                    if (j != 1) query += " AND ";
                    query += "( ";

                    for (var i = 0; i < item2.Count; ++i)
                    {
                        if (i != 0) query += "OR ";
                        query += "Word" + j + "='" + item2[i].ChangeSpecialCharacters() + "' ";
                    }

                    query += ")";
                    ++j;
                }
                query += " )";
                ++z;
            }

            query += ";";

            return query;
        }

        #endregion

        #region PRIVATE
        private string QueryCreator(int ngramSize, int numberComparedWords, IReadOnlyList<string> wordList)
        {
            var query = "SELECT * FROM " + _dbTableDbTableName[ngramSize - 1] + " WHERE";

            for (var i = 0; i < numberComparedWords; ++i)
            {
                if (i != 0) query += " AND";
                query += " Word" + (i + 1) + "='" + wordList[i].ChangeSpecialCharacters() + "'";
            }

            query += ";";

            return query;
        }

        private int GetIndexOfNames(string str)
        {
            string tmp;
            switch (str[0])
            {
                case 'ą':
                    tmp = "a";
                    break;
                case 'ć':
                    tmp = "c";
                    break;
                case 'ę':
                    tmp = "e";
                    break;
                case 'ł':
                    tmp = "l";
                    break;
                case 'ń':
                    tmp = "n";
                    break;
                case 'ó':
                    tmp = "o";
                    break;
                case 'ś':
                    tmp = "s";
                    break;
                case 'ż':
                case 'ź':
                    tmp = "z";
                    break;
                case char n when (n >= 'a' && n <= 'z'):
                    tmp = n.ToString();
                    break;
                default:
                    tmp = "other";
                    break;
            }

            var index = -1;
            for (var i = 0; i < Names.Length; i++)
            {
                if (!Names[i].Equals(tmp)) continue;
                index = i;
                break;
            }

            return index;
        }
        #endregion
    }
}

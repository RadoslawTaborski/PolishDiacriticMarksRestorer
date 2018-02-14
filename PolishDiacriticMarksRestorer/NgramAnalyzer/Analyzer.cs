using System;
using System.Collections.Generic;
using System.Linq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer
{
    /// <summary>
    /// Analyze class compare data form database and input and provides result of Analyze.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IAnalyzer" />
    public class Analyzer : IAnalyzer
    {
        #region FIELDS
        private IDataAccess _db;
        private IQueryProvider _queryProvider;
        private NgramType _ngramType;
        #endregion

        #region CONSTRUCTORS

        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method set a IDataAccess.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <inheritdoc />
        public void SetData(IDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// This method set a NgramType.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <inheritdoc />
        public void SetNgram(NgramType type)
        {
            _ngramType = type;
        }

        /// <summary>
        /// This method set a IQueryProvider.
        /// </summary>
        /// <param name="queryProvider">The query provider.</param>
        /// <inheritdoc />
        public void SetQueryProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        /// <summary>
        /// This method analyze correctness input.
        /// </summary>
        /// <param name="strArray">Array of strings to analyze.</param>
        /// <returns>
        /// String array with result of analyze.
        /// </returns>
        /// <inheritdoc />
        public List<string> AnalyzeStrings(List<string> strArray)
        {
            var length = (int)_ngramType;
            Console.WriteLine("length: " +length);
            Console.WriteLine("count: " + strArray.Count);
            for (var i = length - 1; i < strArray.Count; ++i)
            {
                Console.WriteLine("i: " + i);
                var adder = new DiacriticMarksAdder();
                var combinations = adder.Start(strArray[i], 100);
                var words = (from kvp in combinations select kvp.Key).Distinct().ToList();
                var unigrams = CheckWords(words);
                words = (from ngram in unigrams select ngram.WordsList[0]).Distinct().ToList();

                if (words.Count == 1)
                {
                    strArray[i] = words[0];
                    continue;
                }

                var ngrams = GetData(strArray.Skip(i - length + 1).Take(length - 1).ToList(), words);

                if (ngrams.Count() == 1)
                {
                    strArray[i] = ngrams[0].WordsList[ngrams[0].WordsList.Count-1];
                    continue;
                }

                strArray[i] = words[0];
            }

            return strArray;
        }
        #endregion

        #region PRIVATE
        private List<NGram> GetData(List<string> str, List<string> combinations)
        {
            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.GetMultiNgramsFromTable(_ngramType, str, combinations));
            _db.Disconnect();

            var ngramsList = new List<NGram>();
            for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
            {
                var dataRow = data.Tables[0].Rows[i].ItemArray;
                ngramsList.Add(StringArrayToNgram(dataRow.Select(item => item.ToString()).ToArray()));
            }

            return ngramsList;
        }

        private IEnumerable<NGram> CheckWords(List<string> str)
        {
            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.CheckWordsInUnigramFromTable(str.ToList()));
            _db.Disconnect();

            var ngramsList = new List<NGram>();
            for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
            {
                var dataRow = data.Tables[0].Rows[i].ItemArray;
                ngramsList.Add(StringArrayToNgram(dataRow.Select(item => item.ToString()).ToArray()));
            }

            return ngramsList;
        }

        private NGram StringArrayToNgram(string[] strArray)
        {
            var ngram = new NGram();
            var good = int.TryParse(strArray[1], out ngram.Value);
            ngram.WordsList = new List<string>();

            if (!good) return ngram;

            foreach (var item in strArray.Skip(2))
            {
                ngram.WordsList.Add(item);
            }

            return ngram;
        }
        #endregion
    }
}

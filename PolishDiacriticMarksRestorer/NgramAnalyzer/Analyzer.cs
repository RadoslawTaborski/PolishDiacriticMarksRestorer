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
        public string[] AnalyzeStrings(string[] strArray)
        {
            var adder = new DiacriticMarksAdder();
            var combinations = adder.Start(strArray[2], 100);
            var words = (from kvp in combinations select kvp.Key).Distinct().ToList();
            var unigrams = CheckWords(words);
            words = (from ngram in unigrams select ngram.WordsList[0]).Distinct().ToList();

            switch (words.Count)
            {
                case 1:
                    var list = strArray.ToList();
                    list[list.Count - 1] = words[0];
                    return list.ToArray();
                case 0:
                    return strArray;
            }

            var ngrams = GetData(strArray.Take(strArray.Length - 1).ToList(), words);

            var nGrams = ngrams as NGram[] ?? ngrams.ToArray();

            if (nGrams.Any()) return nGrams[0].ToStrings();

            var list2 = strArray.ToList();
            list2[list2.Count - 1] = words[0];
            return list2.ToArray();
        }
        #endregion

        #region PRIVATE
        private IEnumerable<NGram> GetData(List<string> str, List<string> combinations)
        {
            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.GetMultiNgramsFromTable(_ngramType, str, combinations));
            _db.Disconnect();

            var ngramsList = new List<NGram>();
            for (var i =0; i<data.Tables[0].Rows.Count; ++i)
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

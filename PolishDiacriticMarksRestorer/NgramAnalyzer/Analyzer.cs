using System.Collections.Generic;
using System.Linq;
using MoreLinq;
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
        private readonly IDiacriticMarksAdder _diacriticAdder;
        private NgramType _ngramType;

        public Analyzer(IDiacriticMarksAdder diacriticAdder)
        {
            _diacriticAdder = diacriticAdder;
        }
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
        /// <param name="strList">Array of strings to analyze.</param>
        /// <returns>
        /// String array with result of analyze.
        /// </returns>
        /// <inheritdoc />
        public List<string> AnalyzeStrings(List<string> strList)
        {
            var result = new List<string>();
            var wordLists = new List<List<List<string>>>();
            var ngrams = new List<NGram>();

            var length = (int)_ngramType;
            var count = strList.Count / length;
            var flag = strList.Count - count * length;

            if (strList.Count >= length)
            {
                for (var j = 0; j < count; j++)
                {
                    var tmp2 = strList.GetRange(j * length, length).ConvertAll(d => d.ToLower());
                    ngrams.Add(new NGram { Value = 0, WordsList = tmp2 });
                    var tmp = GetCombinationsList(tmp2);
                    wordLists.Add(tmp);
                }

                if (flag > 0)
                {
                    var tmp2 = strList.GetRange(strList.Count - length, length).ConvertAll(d => d.ToLower());
                    ngrams.Add(new NGram { Value = 0, WordsList = tmp2 });
                    var tmp = GetCombinationsList(tmp2);
                    wordLists.Add(tmp);
                }
            }

            ngrams.AddRange(GetAllData(wordLists));

            foreach (var item in wordLists)
            {
                var concreteNgrams = FindInNgramsList(_ngramType, item, ngrams);
                var ngram = concreteNgrams.MaxBy(x => x.Value);
                result.AddRange(ngram.WordsList);
            }

            for (var i = 0; i < length - flag; i++)
            {
                result.RemoveAt(result.Count - 2);
            }

            return result;
        }
        #endregion

        #region PRIVATE
        private List<NGram> FindInNgramsList(NgramType type, List<List<string>> words, List<NGram> ngrams)
        {
            var length = (int)type;

            var result = new List<NGram>();
            foreach (var item in ngrams)
            {
                var flag = false;
                for (var i = 0; i < length; i++)
                {
                    flag = false;
                    var i1 = i;
                    var matchingvalues = words[i].Where(stringToCheck => stringToCheck.Contains(item.WordsList[i1]));
                    if (!matchingvalues.Any()) break;
                    flag = true;
                }
                if (flag)
                    result.Add(item);
            }

            return result;
        }

        private List<List<string>> GetCombinationsList(List<string> words2)
        {
            var result = new List<List<string>>();

            foreach (var item in words2)
            {
                var combinations = _diacriticAdder.Start(item, 100);
                var words = (from kvp in combinations select kvp.Key).Distinct().ToList();
                var unigrams = CheckWords(words);
                words = (from ngram in unigrams select ngram.WordsList[0]).Distinct().ToList();
                if (words.Count == 0)
                    words.Add(item);
                result.Add(words);
            }

            return result;
        }

        //private List<NGram> GetData(List<string> str, List<string> combinations)
        //{
        //    _db.ConnectToDb();
        //    var data = _db.ExecuteSqlCommand(_queryProvider.GetMultiNgramsFromTable(_ngramType, str, combinations));
        //    _db.Disconnect();

        //    var ngramsList = new List<NGram>();
        //    for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
        //    {
        //        var dataRow = data.Tables[0].Rows[i].ItemArray;
        //        var ngram = StringArrayToNgram(dataRow.Select(item => item.ToString()).ToArray());
        //        if (ngram != null)
        //            ngramsList.Add((NGram)ngram);
        //    }

        //    return ngramsList;
        //}

        private List<NGram> GetAllData(List<List<List<string>>> wordLists)
        {
            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.GetAllNecessaryNgramsFromTable(_ngramType, wordLists));
            _db.Disconnect();

            var ngramsList = new List<NGram>();
            for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
            {
                var dataRow = data.Tables[0].Rows[i].ItemArray;
                var ngram = StringArrayToNgram(dataRow.Select(item => item.ToString()).ToArray());
                if (ngram != null)
                    ngramsList.Add((NGram)ngram);
            }

            return ngramsList;
        }

        private IEnumerable<NGram> CheckWords(List<string> str)
        {
            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.CheckWordsInUnigramFromTable(str));
            _db.Disconnect();

            var ngramsList = new List<NGram>();
            for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
            {
                var dataRow = data.Tables[0].Rows[i].ItemArray;
                var ngram = StringArrayToNgram(dataRow.Select(item => item.ToString()).ToArray());
                if (ngram != null)
                    ngramsList.Add((NGram)ngram);
            }

            return ngramsList;
        }

        private NGram? StringArrayToNgram(string[] strArray)
        {
            var ngram = new NGram();
            var good = int.TryParse(strArray[1], out ngram.Value);
            ngram.WordsList = new List<string>();

            if (!good) return null;

            foreach (var item in strArray.Skip(2))
            {
                ngram.WordsList.Add(item);
            }

            return ngram;
        }
        #endregion
    }
}

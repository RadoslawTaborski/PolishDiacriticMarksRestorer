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
            var ngramVariants = new List<NGramVariants>();
            var result = new List<string>();

            var length = (int)_ngramType;
            var count = strList.Count / length;
            var flag = strList.Count - count * length;

            if (strList.Count >= length)
            {
                for (var j = 0; j < count; j++)
                {
                    var tmp = strList.GetRange(j * length, length).ConvertAll(d => d.ToLower());
                    ngramVariants.Add(new NGramVariants(new NGram { Value = 0, WordsList = tmp }, _diacriticAdder));
                }

                if (flag > 0)
                {
                    var tmp = strList.GetRange(strList.Count - length, length).ConvertAll(d => d.ToLower());
                    ngramVariants.Add(new NGramVariants(new NGram { Value = 0, WordsList = tmp }, _diacriticAdder));
                }
            }

            var list = new List<string>();
            foreach (var variant in ngramVariants)
            {
                variant.CreateVariants();
                list.AddRange(variant.VariantsToWordList());
            }

            var goodWords = CheckWords(list);

            foreach (var variant in ngramVariants)
            {
                variant.LeaveGoodVariants(goodWords);
            }

            var ngrams = GetAllData(ngramVariants).ToList();

            foreach (var variant in ngramVariants)
            {
                variant.UpdateNGramsVariants(ngrams);
            }

            foreach (var item in ngramVariants)
            {
                if (item.NgramVariants.Count == 0)
                {
                    result.AddRange(item.OrginalNGram.WordsList);
                }
                else
                {
                    var ngram = item.NgramVariants.MaxBy(x => x.Value);
                    result.AddRange(ngram.WordsList);
                }
            }

            for (var i = 0; i < flag; i++)
            {
                result.RemoveAt(result.Count - 2);
            }

            return result;
        }
        #endregion

        #region PRIVATE
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

        private IEnumerable<NGram> GetAllData(List<NGramVariants> wordLists)
        {
            var lists = new List<List<List<string>>>();
            foreach (var item in wordLists)
            {
                var tmp = item.VariantsToStringsLists();
                if (tmp!=null)
                    lists.Add(tmp);
            }

            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.GetAllNecessaryNgramsFromTable(_ngramType, lists));
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

        private List<string> CheckWords(List<string> str)
        {
            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.CheckWordsInUnigramFromTable(str));
            _db.Disconnect();

            var wordsList = new List<string>();
            for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
            {
                var dataRow = data.Tables[0].Rows[i].ItemArray;
                wordsList.Add(dataRow.Select(item => item.ToString()).ToArray()[2]);
            }

            return wordsList;
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

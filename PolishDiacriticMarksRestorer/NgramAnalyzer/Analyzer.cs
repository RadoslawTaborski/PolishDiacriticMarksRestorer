using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            if (strList.Count >= length)
            {
                for (var j = 0; j < strList.Count - length+1; j++)
                {
                    var tmp = strList.GetRange(j, length).ConvertAll(d => d.ToLower());
                    ngramVariants.Add(new NGramVariants(new NGram { Value = 0, WordsList = tmp }, _diacriticAdder));
                }
            }

            var list = new List<string>();
            foreach (var variant in ngramVariants)
            {
                variant.CreateVariants();
                list.AddRange(variant.VariantsToWordList());
            }

            list = list.Distinct().ToList();
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

            var finalVariants = new List<NGram>();
            foreach (var item in ngramVariants)
            {
                if (item.NgramVariants.Count == 0)
                {
                    finalVariants.Add(item.OrginalNGram);
                }
                else
                {
                    var ngram = item.NgramVariants.MaxBy(x => x.Value);
                    finalVariants.Add(ngram);
                }
            }

            for (var i = 0; i < length-1; ++i)
            {
                var tmp = new List<NGram>();
                for (var j = i; j >= 0; --j)
                {
                    tmp.Add(finalVariants[j]);
                }

                result.Add(FindWordFromNgramList(tmp,true));
            }

            for (var i = 0; i < finalVariants.Count-length+1; ++i)
            {
                var tmp = new List<NGram>();
                for (var j = i+length-1; j >= i; --j)
                {
                    tmp.Add(finalVariants[j]);
                }

                result.Add(FindWordFromNgramList(tmp,true));
            }

            for (var i = finalVariants.Count - length+1; i < finalVariants.Count; ++i)
            {
                var tmp = new List<NGram>();
                for (var j = i; j < finalVariants.Count; ++j)
                {
                    tmp.Add(finalVariants[j]);
                }

                result.Add(FindWordFromNgramList(tmp,false));
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
        private string FindWordFromNgramList(List<NGram> ngrams, bool fromBeginning)
        {
            var ngram = ngrams.MaxBy(x => x.Value);
            var index=ngrams.IndexOf(ngram);
            
            return fromBeginning ? ngram.WordsList[index] : ngram.WordsList[ngram.WordsList.Count-index-1];
        }

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

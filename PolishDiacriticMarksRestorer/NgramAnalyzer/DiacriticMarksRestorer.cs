using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NgramAnalyzer.Common;
using NgramAnalyzer.Common.Dictionaries;
using NgramAnalyzer.Common.NgramsConnectors;
using NgramAnalyzer.Interfaces;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer
{
    /// <summary>
    /// Analyze class compare data form database and input and provides result of Analyze.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IAnalyzer" />
    public class DiacriticMarksRestorer : IAnalyzer
    {
        #region FIELDS
        private IDataAccess _db;
        private IQueryProvider _queryProvider;
        private readonly IDictionary _dictionary;
        private readonly IFragmentsSplitter _splitter;
        private readonly ILetterChanger _diacriticAdder;
        private NgramType _ngramType;
        private readonly ICharactersIgnorer _iManager;
        private readonly INgramsConnector _ngramConnector;

        public List<string> Input { get; private set; }
        public List<string> InputWithWhiteMarks { get; private set; }
        public List<string> Output { get; private set; }
        // public List<string> WordsCombinations { get; private set; }
        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the <see cref="DiacriticMarksRestorer"/> class.
        /// </summary>
        /// <param name="diacriticAdder">The diacritic adder.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="splitter"></param>
        /// <param name="iManager"></param>
        /// <param name="ngramConnector"></param>
        public DiacriticMarksRestorer(ILetterChanger diacriticAdder=null, IDictionary dictionary=null, IFragmentsSplitter splitter=null, ICharactersIgnorer iManager=null, INgramsConnector ngramConnector=null)
        {
            _diacriticAdder = diacriticAdder ?? new DiacriticMarksAdder();
            if(dictionary == null)
            {
                var logFile = File.ReadAllLines(@"Resources\dictionary");
                var logList = new List<string>(logFile);
                var result = new Dictionary<string, int>();
                foreach (var item in logList)
                {
                    result.Add(item, 0);
                }
                dictionary= new Dict(result);
            }

            _dictionary = dictionary;
            _splitter = splitter;
            _ngramConnector = ngramConnector ?? new Hierarchy();
            _iManager = iManager ;
            _ngramType = NgramType.Bigram;
        }
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

        public int SetWords(string text)
        {
            Input = TextSpliter.Split(text).ToList();
            InputWithWhiteMarks = TextSpliter.SplitAndKeep(text).ToList();
            return Input.Count();
        }

        public List<Tuple<List<string>,bool>> CreateWordsCombinations()
        {
            var result = new List<Tuple<List<string>, bool>>();

            foreach (var item in InputWithWhiteMarks)
            {
                var flag = false;
                if (TextSpliter.Delimiters.Any(elem => item == elem))
                {
                    result.Add(new Tuple<List<string>, bool>(new List<string>() { item }, true));
                    flag = true;
                }

                if (flag) continue;
                var list = CreateCombinationsWord(item);
                var tmp = _dictionary.CheckWords(list);
                var results = false;
                if (tmp.Count != 0)
                {
                    list = tmp;
                    results = true;
                }

                RestoreUpperLetter(item, ref list);
                result.Add(new Tuple<List<string>, bool>(new List<string>(list), results));
            }

            return result;
        }

        /// <summary>
        /// This method analyze correctness input.
        /// </summary>
        /// <param name="str">text to analyze.</param>
        /// <param name="times"></param>
        /// <param name="counts"></param>
        /// <returns>
        /// String array with result of analyze toghether with white marks.
        /// </returns>
        /// <inheritdoc />
        public List<string> AnalyzeText(string str, out List<TimeSpan> times, out List<int> counts)
        {
            counts = new List<int>();
            for (var i = 0; i < 9; i++)
            {
                counts.Add(0);
            }

            times = new List<TimeSpan>();
            for (var i = 0; i < 10; ++i)
            {
                times.Add(DateTime.Now - DateTime.Now);
            }

            if (Input == null)
                SetWords(str);

            counts[0] = Input.Count;

            var length = (int)_ngramType;
            var start = DateTime.Now;
            var sentences = _splitter != null ? _splitter.Split(Input) : new List<Sentence> { new Sentence(Input, "") };
            counts[1] = sentences.Count;
            var stop = DateTime.Now;
            times[0] = stop - start;
            Output = new List<string>();
            foreach (var sentence in sentences)
            {
                if (sentence.Text.Count < length)
                {
                    var type = (NgramType)sentence.Text.Count;
                    Output.AddRange(AnalyzeSentence(sentence, type, ref times, ref counts));
                    continue;
                }

                Output.AddRange(AnalyzeSentence(sentence, _ngramType, ref times, ref counts));
            }

            start = DateTime.Now;
            var result = ReturnForm(InputWithWhiteMarks, Output);
            stop = DateTime.Now;
            times[8] = stop - start;

            return result;
        }
        #endregion

        #region PRIVATE

        private List<string> AnalyzeSentence(Sentence sentence, NgramType type, ref List<TimeSpan> times, ref List<int> counts)
        {
            var length = (int)type;
            var result = new List<string>();
            var ngramVariants = new List<NGramVariants>();

            var start = DateTime.Now;
            var combinationWords = CreateCombinationsWordList(sentence.Text);
            counts[2] += combinationWords.Count;
            var stop = DateTime.Now;
            times[1] += stop - start;

            start = DateTime.Now;
            combinationWords = _dictionary.CheckWords(combinationWords);
            stop = DateTime.Now;
            counts[3] += combinationWords.Count;
            times[2] += stop - start;

            start = DateTime.Now;
            ngramVariants.AddRange(CreateNgramVariantsList(sentence.Text, combinationWords, length));
            stop = DateTime.Now;
            counts[4] += ngramVariants.Count;
            times[3] += stop - start;

            start = DateTime.Now;
            var tmp = ngramVariants.Where(x => x.NgramVariants.Count > 1).ToList();
            stop = DateTime.Now;
            counts[5] += tmp.Count;
            times[4] += stop - start;

            foreach (var item in tmp)
            {
                counts[6] += item.NgramVariants.Count;
            }
            start = DateTime.Now;
            var ngrams = GetAllData(tmp, type).ToList();
            //var ngrams = new List<NGram>();
            stop = DateTime.Now;
            counts[7] += ngrams.Count;
            times[5] += stop - start;

            start = DateTime.Now;
            foreach (var variant in ngramVariants)
            {
                variant.UpdateNGramsVariants(ngrams);
                variant.RestoreUpperLettersInVariants();
                variant.CountProbability(tmp.Contains(variant));
            }
            stop = DateTime.Now;
            times[6] += stop - start;

            start = DateTime.Now;
            result.AddRange(_ngramConnector.AnalyzeNgramsVariants(ngramVariants, length, sentence.Text.Count));
            result[result.Count - 1] = result[result.Count - 1] + sentence.EndMarks;
            counts[8] += result.Count;
            stop = DateTime.Now;
            times[7] += stop - start;

            return result;
        }

        private IEnumerable<NGram> GetAllData(List<NGramVariants> wordLists, NgramType type)
        {
            var ngramsList = new List<NGram>();
            var ngramVariants = new List<NGram>();

            foreach (var item in wordLists)
            {
                foreach (var elem in item.NgramVariants)
                {
                    ngramVariants.Add(elem.Ngram);
                }
            }

            ngramVariants = ngramVariants.Distinct().ToList();
            var index = ngramVariants.FindIndex(x => x.WordsList.Contains(""));
            if (index != -1)
            {
                ngramVariants.RemoveAt(index);
            }

            _db.ConnectToDb();
            foreach (var elem in ngramVariants)
            {
                var tmp = new NGram(elem);

                if (_iManager != null)
                    tmp = _iManager.Remove(tmp);

                var data = _db.ExecuteSqlCommand(_queryProvider.GetTheSameNgramsFromTable(type, tmp.WordsList));
                for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
                {
                    var dataRow = data.Tables[0].Rows[i].ItemArray;
                    var ngram = StringArrayToNgram(dataRow.Select(a => a.ToString()).ToArray());
                    if (ngram == null) continue;
                    var n = (NGram)ngram;
                    if (_iManager != null)
                        n=_iManager.Restore(elem, n);
                    ngramsList.Add(n);
                }

            }
            _db.Disconnect();

            return ngramsList;
        }

        private NGram? StringArrayToNgram(string[] strArray)
        {
            var good = int.TryParse(strArray[1], out var value);

            if (!good) return null;

            var list = strArray.Skip(2).ToList();

            var ngram = new NGram(value, list);
            return ngram;
        }

        private List<string> CreateCombinationsWordList(List<string> strList)
        {
            var words = new List<string>();
            foreach (var str in strList)
            {
                var combinations = CreateCombinationsWord(str);
                words.AddRange(combinations);
            }

            words = words.Distinct().ToList();
            return words;
        }

        private List<string> CreateCombinationsWord(string word)
        {
            var combinations = _diacriticAdder.Start(word.ToLower(), 100);
            var result = (from kvp in combinations select kvp.Key).Distinct().ToList();

            return result;
        }

        private void RestoreUpperLetter(string original, ref List<string> combinations)
        {
            var r = new Regex(@"[A-ZĄĆĘŁŃŚÓŹŻ]");

            for (var j = 0; j < original.Length; ++j)
            {
                if (!r.IsMatch(original[j].ToString())) continue;
                for (var i = 0; i < combinations.Count; ++i)
                {
                    var strBuilder =
                        new System.Text.StringBuilder(combinations[i])
                        {
                            [j] = char.ToUpper(combinations[i][j])
                        };
                    combinations[i] = strBuilder.ToString();
                }
            }
        }

        private List<NGramVariants> CreateNgramVariantsList(List<string> strList, List<string> dictionary, int length)
        {
            var ngramVariants = new List<NGramVariants>();

            for (var j = 0; j < strList.Count - length + 1; j++)
            {
                var tmp = strList.GetRange(j, length);
                ngramVariants.Add(new NGramVariants(new NGram(0, tmp), _diacriticAdder));
                ngramVariants[ngramVariants.Count - 1].CreateVariants(dictionary);
            }

            return ngramVariants;
        }

        private List<string> ReturnForm(List<string> older, List<string> newer)
        {
            var copy = new List<string>(older);
            var result = new List<string>();

            for (var i = 0; i < copy.Count(); ++i)
            {
                var flag = TextSpliter.Delimiters.Any(elem => copy[i] == elem);
                if (!flag)
                {
                    copy[i] = "X";
                }
            }

            var index = 0;
            for (var i = 0; i < copy.Count(); ++i)
            {
                if (copy[i] == "X")
                {
                    result.Add(newer[index]);
                    ++index;
                }
                else
                {
                    result.Add(copy[i]);
                }
            }

            return result;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly IDictionary _dictionary;
        private readonly ISentenceSpliter _spliter;
        private readonly IDiacriticMarksAdder _diacriticAdder;
        private NgramType _ngramType;
        private readonly bool _ignorePunctationMarksInNgrams;
        private readonly List<string> _preposition = new List<string>
        {
            //"o",
            //"w",
            //"we",
            "z",
            "ze",
            //"na",
            //"od",
            //"ode",
            //"do",
            "za",
            //"po",
            //"dla",
            //"bez",
            //"beze",
            "pod",
            "pode",
            "nad",
            "nade",
            //"obok",
            //"przy",
            //"przez",
            //"przeze",
            "przed",
            "przede",
            "między",
            "pomiędzy",
            //"według",
            //"zamiast",
            //"podczas",
            "ja"
        };

        public List<string> Input { get; private set; }
        public List<string> InputWithWhiteMarks { get; private set; }
        public List<string> Output { get; private set; }
        // public List<string> WordsCombinations { get; private set; }
        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the <see cref="Analyzer"/> class.
        /// </summary>
        /// <param name="diacriticAdder">The diacritic adder.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="spliter"></param>
        /// <param name="ignorePunctationMarksInNgrams"></param>
        public Analyzer(IDiacriticMarksAdder diacriticAdder, IDictionary dictionary, ISentenceSpliter spliter, bool ignorePunctationMarksInNgrams)
        {
            _diacriticAdder = diacriticAdder;
            _dictionary = dictionary;
            _spliter = spliter;
            _ignorePunctationMarksInNgrams = ignorePunctationMarksInNgrams;
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
        public List<string> AnalyzeString(string str, out List<TimeSpan> times, out List<int> counts)
        {
            counts = new List<int>();
            for (var i = 0; i < 7; i++)
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

            var length = (int)_ngramType;
            var start = DateTime.Now;
            var sentences = _spliter != null ? _spliter.Split(Input) : new List<Sentence> { new Sentence(Input, "") };
            counts[0] = sentences.Count;
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
            counts[1] += combinationWords.Count;
            var stop = DateTime.Now;
            times[1] += stop - start;

            start = DateTime.Now;
            combinationWords = _dictionary.CheckWords(combinationWords);
            stop = DateTime.Now;
            counts[2] += combinationWords.Count;
            times[2] += stop - start;

            start = DateTime.Now;
            ngramVariants.AddRange(CreateNgramVariantsList(sentence.Text, combinationWords, length));
            stop = DateTime.Now;
            counts[3] += ngramVariants.Count;
            times[3] += stop - start;

            start = DateTime.Now;
            var tmp = ngramVariants.Where(x => x.NgramVariants.Count > 1).ToList();
            stop = DateTime.Now;
            counts[4] += tmp.Count;
            times[4] += stop - start;

            start = DateTime.Now;
            var ngrams = GetAllData(tmp, type, _ignorePunctationMarksInNgrams).ToList();
            stop = DateTime.Now;
            counts[5] += ngrams.Count;
            times[5] += stop - start;

            start = DateTime.Now;
            foreach (var variant in ngramVariants)
            {
                variant.UpdateNGramsVariants(ngrams);
                variant.RestoreUpperLettersInVariants();
            }
            stop = DateTime.Now;
            times[6] += stop - start;

            start = DateTime.Now;
            result.AddRange(AnalyzeNgramsVariants2(ngramVariants, length, sentence.Text.Count));
            result[result.Count - 1] = result[result.Count - 1] + sentence.EndMarks;
            counts[6] = result.Count;
            stop = DateTime.Now;
            times[7] += stop - start;

            return result;
        }

        private string FindWordFromNgramList(List<NGram> ngrams)
        {
            var ngram = ngrams.MaxBy(x => x.Value).ElementAt(0);
            var index = ngrams.IndexOf(ngram);

            return ngram.WordsList[ngram.WordsList.Count - index - 1];
        }

        private IEnumerable<NGram> GetAllData(List<NGramVariants> wordLists, NgramType type, bool ignorePunctationMarks)
        {
            var ngramsList = new List<NGram>();
            var ngramVariants = new List<NGram>();

            foreach (var item in wordLists)
            {
                foreach (var elem in item.NgramVariants)
                {
                    ngramVariants.Add(elem);
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

                if (ignorePunctationMarks)
                    for (var i = 0; i < tmp.WordsList.Count; i++)
                    {
                        tmp.WordsList[i] = tmp.WordsList[i].WithoutPunctationMarks();
                    }

                var data = _db.ExecuteSqlCommand(_queryProvider.GetTheSameNgramsFromTable(type, tmp.WordsList));
                for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
                {
                    var dataRow = data.Tables[0].Rows[i].ItemArray;
                    var ngram = StringArrayToNgram(dataRow.Select(a => a.ToString()).ToArray());
                    if (ngram == null) continue;
                    var n = (NGram)ngram;
                    if (ignorePunctationMarks)
                        RestorePunctationMarks(n, elem);
                    ngramsList.Add(n);
                }

            }
            _db.Disconnect();

            return ngramsList;
        }

        private void RestorePunctationMarks(NGram ngram, NGram orginal)
        {
            for (var i = 0; i < orginal.WordsList.Count; i++)
            {
                var item = orginal.WordsList[i];
                var indexes = new List<int>();
                indexes.AddRange(item.AllIndexesOf("."));
                indexes.AddRange(item.AllIndexesOf(","));
                indexes.AddRange(item.AllIndexesOf("?"));
                indexes.AddRange(item.AllIndexesOf("!"));
                indexes.AddRange(item.AllIndexesOf(";"));
                indexes.AddRange(item.AllIndexesOf("("));
                indexes.AddRange(item.AllIndexesOf(")"));
                indexes.AddRange(item.AllIndexesOf(":"));
                indexes.AddRange(item.AllIndexesOf("\""));
                indexes.AddRange(item.AllIndexesOf("'"));
                indexes.AddRange(item.AllIndexesOf("”"));
                indexes.AddRange(item.AllIndexesOf("„"));
                indexes.Sort();

                foreach (var ind in indexes)
                {
                    ngram.WordsList[i] = ngram.WordsList[i].Insert(ind, orginal.WordsList[i][ind].ToString());
                }
            }
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

        private List<NGram> FindTheBestNgramFromNgramsVariants(List<NGramVariants> variants)
        {
            var finalVariants = new List<NGram>();
            foreach (var item in variants)
            {
                var ngram = item.NgramVariants.MaxBy(x => x.Value).ElementAt(0);
                finalVariants.Add(ngram);
            }

            return finalVariants;
        }

        private List<string> AnalyzeNgramsVariants(List<NGramVariants> ngramsVar, int length, int countWords)
        {
            var result = new List<string>();

            var finalVariants = FindTheBestNgramFromNgramsVariants(ngramsVar);

            for (var i = 0; i < countWords; ++i)
            {
                var tmp = CreateFrame(finalVariants, length, i);
                result.Add(FindWordFromNgramList(tmp));
            }

            return result;
        }

        private List<string> AnalyzeNgramsVariants2(List<NGramVariants> ngramsVar, int length, int countWords)
        {
            var result = new List<string>();

            var finalVariants = new NGram[ngramsVar.Count];
            for (var index = 0; index < ngramsVar.Count; index++)
            {
                if (ngramsVar[index].NgramVariants.Count == 1)
                    finalVariants[index] = new NGram(int.MaxValue, ngramsVar[index].NgramVariants[0].WordsList);
            }

            var idx = new List<int>();
            for (var i = 0; i < ngramsVar.Count; ++i)
            {
                foreach (var item in ngramsVar[i].OrginalNGram.WordsList.Take(ngramsVar[i].OrginalNGram.WordsList.Count-1))
                {
                    if(ngramsVar[i].NgramVariants.Count<2) continue;
                    var flag = false;
                    foreach (var prep in _preposition)
                    {
                        if (!item.WithoutPunctationMarks().Equals(prep)) continue;
                        idx.Add(i);
                        flag = true;
                        break;
                    }

                    if (flag) break;
                }
            }

            var param2 = 2;
            var sorted = ngramsVar
                .Select((x, i) => new KeyValuePair<double, int>(x.NgramVariants.MaxBy(a => a.Value).ElementAt(0).Value, i))
                .OrderBy(x => x.Key)
                .ToList();

            var aa = sorted.Where(x => x.Key > param2);
            var keyValuePairs = aa as KeyValuePair<double, int>[] ?? aa.ToArray();
            if (keyValuePairs.Any())
            {
                var ee = keyValuePairs.ElementAt(0);
                var ii = sorted.IndexOf(ee);
                var sublist = sorted.GetRange(0, ii);
                var sublist2 = sorted.GetRange(ii, sorted.Count() - sublist.Count);
                sublist.Reverse();
                sublist2.AddRange(sublist);
                sorted = sublist2;
            }

            var idx2 = sorted.Select(x => x.Value).ToList();
            idx.AddRange(idx2);

            for (var i = 0; i < idx.Count; i++)
            {
                if (finalVariants[idx[i]].WordsList != null) continue;

                var tmp = ngramsVar[idx[i]].NgramVariants;
                if (idx[i] - 1 >= 0)
                    if (finalVariants[idx[i] - 1].WordsList != null)
                    {
                        tmp = tmp.Where(x => x.WordsList[0] == finalVariants[idx[i] - 1].WordsList[1]).ToList();
                    }
                if (idx[i] + 1 < ngramsVar.Count && tmp.Count > 1)
                    if (finalVariants[idx[i] + 1].WordsList != null)
                    {
                        tmp = tmp.Where(x => x.WordsList[1] == finalVariants[idx[i] + 1].WordsList[0]).ToList();
                    }

                if (tmp.Count > 0)
                {
                    var ngram = tmp.MaxBy(x => x.Value).ElementAt(0);
                    finalVariants[idx[i]] = ngram;
                }
                else
                {
                    var ngram = ngramsVar[i].NgramVariants.MaxBy(x => x.Value).ElementAt(0);
                    finalVariants[idx[i]] = ngram;
                }
            }


            for (var i = 0; i < countWords; ++i)
            {
                var tmp = CreateFrame(finalVariants.ToList(), length, i);
                result.Add(FindWordFromNgramList(tmp));
            }

            return result;
        }

        private List<NGram> CreateFrame(List<NGram> ngrams, int length, int indexOfWord)
        {
            var result = new List<NGram>();

            for (var i = indexOfWord - length + 1; i <= indexOfWord; ++i)
            {
                if (i < 0 || i >= ngrams.Count)
                    result.Add(new NGram(-1, new List<string>()));
                else
                    result.Add(ngrams[i]);
            }

            return result;
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

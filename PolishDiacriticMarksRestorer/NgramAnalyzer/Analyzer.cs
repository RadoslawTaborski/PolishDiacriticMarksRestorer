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
    public class Analyzer : Interfaces.IAnalyzer
    {
        #region FIELDS
        private IDataAccess _db;
        private IQueryProvider _queryProvider;
        private readonly IDictionary _dictionary;
        private readonly ISentenceSpliter _spliter;
        private readonly IDiacriticMarksAdder _diacriticAdder;
        private NgramType _ngramType;

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
        public Analyzer(IDiacriticMarksAdder diacriticAdder, IDictionary dictionary, ISentenceSpliter spliter)
        {
            _diacriticAdder = diacriticAdder;
            _dictionary = dictionary;
            _spliter = spliter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Analyzer"/> class.
        /// </summary>
        /// <param name="diacriticAdder">The diacritic adder.</param>
        /// <param name="spliter"></param>
        public Analyzer(IDiacriticMarksAdder diacriticAdder, ISentenceSpliter spliter)
        {
            _diacriticAdder = diacriticAdder;
            _spliter = spliter;
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

        public List<List<string>> CreateWordsCombinations()
        {
            var result = new List<List<string>>();

            foreach (var item in InputWithWhiteMarks)
            {
                var flag = false;
                if (TextSpliter.Delimiters.Any(elem => item == elem))
                {
                    result.Add(new List<string>() { item });
                    flag = true;
                }

                if (flag) continue;
                var list = CreateCombinationsWord(item);
                list = _dictionary != null ? _dictionary.CheckWords(list) : CheckWords(list);
                RestoreUpperLetter(item, ref list);
                result.Add(list);
            }

            return result;
        }

        /// <summary>
        /// This method analyze correctness input.
        /// </summary>
        /// <param name="str">text to analyze.</param>
        /// <returns>
        /// String array with result of analyze toghether with white marks.
        /// </returns>
        /// <inheritdoc />
        public List<string> AnalyzeString(string str)
        {
            DateTime start, stop;
            TimeSpan time;
            if (Input == null)
                SetWords(str);

            //Console.WriteLine("Start");
            var length = (int)_ngramType;

            var sentences = _spliter != null ? _spliter.Split(Input) : new List<Sentence> { new Sentence(Input, "") };

            Output = new List<string>();
            foreach (var sentence in sentences)
            {
                var ngramVariants = new List<NGramVariants>();
                if (sentence.Text.Count < length) {
                    Output.AddRange(sentence.Text);
                    Output[Output.Count - 1] = Output[Output.Count - 1] + sentence.EndMarks;
                    //Console.WriteLine(sentence);
                    continue;
                }   
                start = DateTime.Now;
                var combinationWords = CreateCombinationsWordList(sentence.Text);
                stop = DateTime.Now;
                time = stop - start;
                //Console.WriteLine($"Czas generowania kombinacji słów: {new DateTime(time.Ticks):HH:mm:ss.f}");

                start = DateTime.Now;
                combinationWords = _dictionary != null ? _dictionary.CheckWords(combinationWords) : CheckWords(combinationWords);
                stop = DateTime.Now;
                time = stop - start;
                //Console.WriteLine($"Czas sprawdzania słów: {new DateTime(time.Ticks):HH:mm:ss.f}");

                start = DateTime.Now;
                ngramVariants.AddRange(CreateNgramVariantsList(sentence.Text, combinationWords, length));
                stop = DateTime.Now;
                time = stop - start;
                //Console.WriteLine($"Czas generowania kombinacji ngramów: {new DateTime(time.Ticks):HH:mm:ss.f}");

                var ngrams = new List<NGram>();
                stop = DateTime.Now;
                time = stop - stop;
                for (var i = 0; i < 1; ++i)
                {
                    start = DateTime.Now;
                    ngrams = GetAllData(ngramVariants).ToList();
                    stop = DateTime.Now;
                    time += stop - start;
                }
                //Console.WriteLine($"Czas pobierania danych SINGLE QUERY: {new DateTime(time.Ticks / 20):HH:mm:ss.f}");

                start = DateTime.Now;
                foreach (var variant in ngramVariants)
                {
                    variant.UpdateNGramsVariants(ngrams);
                    variant.RestoreUpperLettersInVariants();
                }
                stop = DateTime.Now;
                time = stop - start;
                //Console.WriteLine($"Czas aktualizowania ngramów: {new DateTime(time.Ticks):HH:mm:ss.f}");

                start = DateTime.Now;
                var finalVariants = FindTheBestNgramFromNgramsVariants(ngramVariants);
                stop = DateTime.Now;
                time = stop - start;
                //Console.WriteLine($"Czas analizowania: {new DateTime(time.Ticks):HH:mm:ss.f}");
                //Console.WriteLine("Koniec");

                Output.AddRange(AnalyzeNgramsList(finalVariants, length, sentence.Text.Count));
                Output[Output.Count - 1] = Output[Output.Count - 1] + sentence.EndMarks;
            }

            return ReturnForm(InputWithWhiteMarks, Output);
        }
        #endregion

        #region PRIVATE
        private string FindWordFromNgramList(List<NGram> ngrams)
        {
            var ngram = ngrams.MaxBy(x => x.Value).ElementAt(0);
            var index = ngrams.IndexOf(ngram);

            return ngram.WordsList[ngram.WordsList.Count - index - 1];
        }

        private IEnumerable<NGram> GetAllData(List<NGramVariants> wordLists)
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
            //Console.WriteLine($"INPUT: {ngramVariants.Count}");

            ngramVariants = ngramVariants.Distinct().ToList();

            _db.ConnectToDb();
            foreach (var elem in ngramVariants)
            {
                var data = _db.ExecuteSqlCommand(_queryProvider.GetTheSameNgramsFromTable(_ngramType, elem.WordsList));
                for (var i = 0; i < data.Tables[0].Rows.Count; ++i)
                {
                    var dataRow = data.Tables[0].Rows[i].ItemArray;
                    var ngram = StringArrayToNgram(dataRow.Select(a => a.ToString()).ToArray());
                    if (ngram != null)
                        ngramsList.Add((NGram)ngram);
                }
            }
            _db.Disconnect();

            //Console.WriteLine($"OUTPUT: {ngramsList.Count}");

            //var sortedList = ngramsList.OrderBy(x => x.Value).ThenBy(x => x.WordsList[0]);
            //foreach (var item in sortedList)
            //    Console.WriteLine(item);

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

        private List<string> AnalyzeNgramsList(List<NGram> ngrams, int length, int countWords)
        {
            var result = new List<string>();

            for (var i = 0; i < countWords; ++i)
            {
                var tmp = CreateFrame(ngrams, length, i);
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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// This class creates, stores and modifies variants of Ngram
    /// </summary>
    public class NGramVariants
    {
        #region FIELDS
        private readonly ILetterChanger _marksAdder;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets the orginal Ngram.
        /// </summary>
        /// <value>
        /// The orginal Ngram.
        /// </value>
        public NGram OrginalNGram { get; }
        /// <summary>
        /// Gets the Ngram variants.
        /// </summary>
        /// <value>
        /// The Ngram variants.
        /// </value>
        public List<NGramVariant> NgramVariants { get; private set; }
        #endregion

        #region CONSTRUCTORS
        internal NGramVariants(NGram orginalNGram, ILetterChanger marksAdder)
        {
            OrginalNGram = orginalNGram;
            _marksAdder = marksAdder;
            NgramVariants = new List<NGramVariant>();
        }
        #endregion

        #region INTERNALS
        /// <summary>
        /// Create the variants.
        /// </summary>
        internal void CreateVariants(List<string> dictionary)
        {
            NgramVariants = new List<NGramVariant>();
            var lists = new List<List<string>>();
            foreach (var item in OrginalNGram.WordsList)
            {
                lists.Add(new List<string>());
                var index = lists.Count - 1;
                var combinations = _marksAdder.Start(item.ToLower(), 100);
                foreach (var combination in combinations)
                {
                    foreach (var word in dictionary)
                    {
                        if (combination.Key == word)
                            lists[index].Add(combination.Key);
                    }
                }

                if (lists[index].Count == 0)
                {
                    lists[index].Add(item.ToLower());
                }
            }

            var res = lists[lists.Count - 1];
            for (var i = lists.Count - 1; i > 0; --i)
            {
                res = Permutation(lists[i - 1], res);
            }

            foreach (var sequence in res)
            {
                NgramVariants.Add(new NGramVariant{Ngram = new NGram(0, sequence.Split(' ').ToList()), Probability = 0});
            }
        }

        /// <summary>
        /// Updates the NgramVariants based on the Ngrams list.
        /// </summary>
        /// <param name="goodNGrams">List with correct Ngrams.</param>
        internal void UpdateNGramsVariants(List<NGram> goodNGrams)
        {
            for (var i = 0; i < NgramVariants.Count; i++)
            {
                foreach (var goodNGram in goodNGrams)
                {
                    if (goodNGram.WordsList.SequenceEqual(NgramVariants[i].Ngram.WordsList))
                    {
                        NgramVariants[i] = new NGramVariant { Ngram = new NGram(goodNGram), Probability = 0 } ;
                    }
                }
            }
        }

        internal void CountProbability(bool check)
        {
            var sum = NgramVariants.Sum(item => item.Ngram.Value);

            for (var i = 0; i < NgramVariants.Count; i++)
            {
                if (sum == 0)
                {
                    if(check)
                        NgramVariants[i] = new NGramVariant {Ngram = NgramVariants[i].Ngram, Probability = 0};
                    else
                        NgramVariants[i] = new NGramVariant { Ngram = NgramVariants[i].Ngram, Probability = 1};
                    continue;
                }

                NgramVariants[i] = new NGramVariant { Ngram = NgramVariants[i].Ngram, Probability = (double)NgramVariants[i].Ngram.Value / sum };
            }
        }

        /// <summary>
        /// Changes NgramVariantes to List of List of words.
        /// </summary>
        /// <returns>List of list of words with NgramVariantes</returns>
        internal List<List<string>> VariantsToStringsLists()
        {
            var result = new List<List<string>>();
            if (NgramVariants.Count == 0)
                return null;
            foreach (var unused in OrginalNGram.WordsList)
            {
                result.Add(new List<string>());
            }

            foreach (var item in NgramVariants)
            {
                for (var i = 0; i < item.Ngram.WordsList.Count; i++)
                {
                    if (!result[i].Contains(item.Ngram.WordsList[i]))
                        result[i].Add(item.Ngram.WordsList[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Restores the upper letters in NgramVariants list.
        /// </summary>
        internal void RestoreUpperLettersInVariants()
        {
            var r = new Regex(@"[A-ZĄĆĘŁŃŚÓŹŻ]");
            for (var i = 0; i < OrginalNGram.WordsList.Count; ++i)
            {
                for (var j = 0; j < OrginalNGram.WordsList[i].Length; ++j)
                {
                    if (!r.IsMatch(OrginalNGram.WordsList[i][j].ToString())) continue;
                    foreach (var ngramVariant in NgramVariants)
                    {
                        var strBuilder =
                            new System.Text.StringBuilder(ngramVariant.Ngram.WordsList[i])
                            {
                                [j] = char.ToUpper(ngramVariant.Ngram.WordsList[i][j])
                            };
                        ngramVariant.Ngram.WordsList[i] = strBuilder.ToString();
                    }
                }
            }
        }

        #endregion

        #region PRIVATE
        private List<string> Permutation(List<string> a, List<string> b)
        {
            var result = new List<string>();
            foreach (var word1 in a)
            {
                foreach (var word2 in b)
                {
                    result.Add(word1 + " " + word2);
                }
            }
            return result;
        }
        #endregion
    }
}

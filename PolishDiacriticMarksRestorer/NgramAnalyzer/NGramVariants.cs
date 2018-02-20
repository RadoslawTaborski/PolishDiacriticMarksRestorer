using System.Collections.Generic;
using System.Linq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer
{
    /// <summary>
    /// This class creates, stores and modifies variants of Ngram
    /// </summary>
    internal class NGramVariants
    {
        #region FIELDS
        private readonly IDiacriticMarksAdder _marksAdder;
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
        public List<NGram> NgramVariants { get; private set; }
        #endregion

        #region CONSTRUCTORS
        internal NGramVariants(NGram orginalNGram, IDiacriticMarksAdder marksAdder)
        {
            OrginalNGram = orginalNGram;
            _marksAdder = marksAdder;
            NgramVariants = new List<NGram>();
        }
        #endregion

        #region INTERNALS
        /// <summary>
        /// Create the variants.
        /// </summary>
        internal void CreateVariants(List<string> dictionary)
        {
            NgramVariants = new List<NGram>();
            var lists = new List<List<string>>();
            foreach (var item in OrginalNGram.WordsList)
            {
                lists.Add(new List<string>());
                var index = lists.Count - 1;
                var combinations = _marksAdder.Start(item, 100);
                foreach (var combination in combinations)
                {
                    foreach (var word in dictionary)
                    {
                        if(combination.Key==word)
                            lists[index].Add(combination.Key);
                    }
                }

                if (lists[index].Count == 0)
                {
                    lists[index].Add(item);
                }
            }

            var res = lists[lists.Count - 1];
            for (var i = lists.Count-1; i > 0; --i)
            {
                res = Permutation(lists[i - 1], res);
            }

            foreach (var sequence in res)
            {
                NgramVariants.Add(new NGram{Value = 0, WordsList = sequence.Split(' ').ToList()});
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
                    if (goodNGram.WordsList.SequenceEqual(NgramVariants[i].WordsList))
                    {
                        NgramVariants[i] = goodNGram;
                    }
                }
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
                for (var i = 0; i < item.WordsList.Count; i++)
                {
                    if (!result[i].Contains(item.WordsList[i]))
                        result[i].Add(item.WordsList[i]);
                }
            }

            return result;
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

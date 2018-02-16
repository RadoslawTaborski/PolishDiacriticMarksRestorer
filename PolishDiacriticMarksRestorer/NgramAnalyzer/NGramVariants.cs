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
        internal void CreateVariants()
        {
            NgramVariants = new List<NGram>();
            var str = "";
            foreach (var item in OrginalNGram.WordsList)
            {
                str += item + " ";
            }

            str = str.Substring(0, str.Length - 1);
            var tmp = _marksAdder.Start(str, 100);

            var list = (from kvp in tmp select kvp.Key).Distinct().ToList();
            foreach (var item in list)
            {
                NgramVariants.Add(new NGram { Value = 0, WordsList = item.Split(' ').ToList() });
            }
        }

        /// <summary>
        /// Changes NgramVariantes to word list.
        /// </summary>
        /// <returns>Word list</returns>
        internal List<string> VariantsToWordList()
        {
            var result = new List<string>();
            foreach (var ngram in NgramVariants)
            {
                foreach (var str in ngram.WordsList)
                {
                    if (!result.Contains(str))
                        result.Add(str);
                }
            }

            return result;
        }

        /// <summary>
        /// Leaves the good variants, but removes incorrect.
        /// </summary>
        /// <param name="goodWords">The good words.</param>
        internal void LeaveGoodVariants(List<string> goodWords)
        {
            for (var i = 0; i < NgramVariants.Count; i++)
            {
                foreach (var item in NgramVariants[i].WordsList)
                {
                    if (goodWords.Contains(item)) continue;

                    NgramVariants.RemoveAt(i);
                    --i;
                    break;
                }
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
    }
}

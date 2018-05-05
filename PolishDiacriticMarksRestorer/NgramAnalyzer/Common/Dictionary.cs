using System.Collections.Generic;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    public class Dict : IDictionary
    {
        #region PROPERTIES

        /// <summary>
        /// Gets the dictionary word list.
        /// </summary>
        /// <value>
        /// Dictionary word list.
        /// </value>
        public Dictionary<string, int> WordList { get; }

        #endregion

        #region CONSTRUCTORS

        public Dict(Dictionary<string, int> list)
        {
            WordList = list;
        }

        #endregion

        #region PUBLIC

        /// <summary>
        /// Checks if the words are in the dictionary.
        /// </summary>
        /// <param name="str">List of strings to investigate.</param>
        /// <returns>List with words from str, which are in dictionary</returns>
        public List<string> CheckWords(List<string> str)
        {
            var result = new List<string>();

            foreach (var word in str)
            {
                if (WordList.ContainsKey(word.WithoutPunctationMarks()))
                    result.Add(word);
            }

            return result;
        }

        /// <summary>
        /// Checks if the word is in the dictionary.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>true if word is in the dictionary.</returns>
        public bool CheckWord(string str)
        {
            var result = WordList.ContainsKey(str.WithoutPunctationMarks());

            return result;
        }

        #endregion
    }
}

using System.Collections.Generic;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    public class Dictionary : IDictionary
    {
        #region PROPERTIES
        /// <summary>
        /// Gets the dictionary word list.
        /// </summary>
        /// <value>
        /// Dictionary word list.
        /// </value>
        public HashSet<string> WordList { get; }
        #endregion

        #region CONSTRUCTORS
        public Dictionary(List<string> list)
        {
            WordList = new HashSet<string>();
            SetWordList(list);
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
                if (WordList.Contains(word.WithoutPunctationMarks()))
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
            return WordList.Contains(str.WithoutPunctationMarks());
        }
        #endregion

        #region PRIVATE
        private void SetWordList(List<string> list)
        {
            foreach (var item in list)
            {
                WordList.Add(item);
            }
        }
        #endregion
    }
}

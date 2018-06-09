using System.Collections.Generic;
using System.Linq;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common.Dictionaries
{
    public class Dict2 : IDictionary
    {
        #region PROPERTIES

        /// <summary>
        /// Gets the dictionary word list.
        /// </summary>
        /// <value>
        /// Dictionary word list.
        /// </value>
        private readonly List<WordMap> _wordList;

        #endregion

        #region CONSTRUCTORS

        public Dict2(Dictionary<string, int> list)
        {
            _wordList=new List<WordMap>();
            foreach (var item in list)
            {
                var tmpStr = item.Key.WithoutPunctationMarks().RemoveDiacritics();
                var tmp = _wordList.FirstOrDefault(i => i.Word == tmpStr);
                if (tmp==null)
                {
                    _wordList.Add(new WordMap(tmpStr));
                    _wordList.Last().Add(item.Key, item.Value);
                }
                else
                {
                    tmp.Add(item.Key, item.Value);
                }
            }
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

            foreach (var item in str)
            {
                var tmp = _wordList.FirstOrDefault(i => i.Word == item.WithoutPunctationMarks().RemoveDiacritics());
                if (tmp != null)
                {
                    result.AddRange(tmp.MappedWords.Keys);
                }
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
            var tmp = _wordList.FirstOrDefault(i => i.Word == str.WithoutPunctationMarks().RemoveDiacritics());
            if (tmp != null)
            {
                if (tmp.MappedWords.ContainsKey(str.WithoutPunctationMarks()))
                    return true;
            }

            return false;
        }

        #endregion
    }
}

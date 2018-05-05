using System.Collections.Generic;

namespace NgramAnalyzer.Interfaces
{
    public interface IDictionary
    {
        Dictionary<string,int> WordList { get; }
            /// <summary>
        /// Checks if the words are in the dictionary.
        /// </summary>
        /// <param name="str">List of strings to investigate.</param>
        /// <returns>List with words from str, which are in dictionary</returns>
        List<string> CheckWords(List<string> str);
        /// <summary>
        /// Checks if the word is in the dictionary.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>true if word is in the dictionary.</returns>
        bool CheckWord(string str);
    }
}

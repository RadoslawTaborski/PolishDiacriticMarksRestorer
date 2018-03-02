using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer
{
    public class DiacriticMarksAdder : IDiacriticMarksAdder
    {
        #region Fields
        private readonly List<KeyValuePair<string, string>> _letterPairs = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("a", "ą"),
            new KeyValuePair<string, string>("c", "ć"),
            new KeyValuePair<string, string>("e", "ę"),
            new KeyValuePair<string, string>("l", "ł"),
            new KeyValuePair<string, string>("n", "ń"),
            new KeyValuePair<string, string>("o", "ó"),
            new KeyValuePair<string, string>("s", "ś"),
            new KeyValuePair<string, string>("z", "ź"),
            new KeyValuePair<string, string>("z", "ż"),
        };
        #endregion

        #region Public
        /// <summary>
        /// Adds diacritics to the word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="howManyChanges">How many add operations.</param>
        /// <returns>
        /// List with pair - word - number of operations
        /// </returns>
        public List<KeyValuePair<string, int>> Start(string word, int howManyChanges)
        {
            var result = new List<KeyValuePair<string, int>> {new KeyValuePair<string, int>(word, 0)};
            foreach (var item in _letterPairs)
            {
                var tmp = new List<KeyValuePair<string, int>>();
                for (var z = 0; z < result.Count; ++z)
                {
                    var copy = result.ElementAt(z);
                    for (var j = 0; j < Math.Pow(Regex.Matches(copy.Key, item.Key).Count, 2); ++j)
                    {
                        var variant = copy;
                        if (variant.Value >= howManyChanges)
                            break;
                        var howMany = 0;
                        var bools = ConvertByteToBoolArray((byte)(j + 1));
                        for (var i = 1; i <= Regex.Matches(copy.Key, item.Key).Count; ++i)
                        {
                            if (bools[i - 1] != true) continue;

                            variant = new KeyValuePair<string, int>(ChangeLetter(variant.Key, item.Key, item.Value, 1, variant.Key.NthIndexOf(item.Key, i - howMany)), variant.Value + 1);
                            ++howMany;
                        }
                        tmp.Add(variant);
                    }
                }
                result.AddRange(tmp);
                result = result.GroupBy(x => x.Key).Select(g => g.Last()).ToList();
            }

            return result;
        }
        #endregion

        #region Private
        private static bool[] ConvertByteToBoolArray(byte b)
        {
            var result = new bool[8];
            for (var i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) != 0;

            return result;
        }

        private string ChangeLetter(string word, string oldStr, string newStr, int count, int position)
        {
            var regex = new Regex(Regex.Escape(oldStr));
            var newText = regex.Replace(word, newStr, count, position);

            return newText;
        }
        #endregion
    }
}

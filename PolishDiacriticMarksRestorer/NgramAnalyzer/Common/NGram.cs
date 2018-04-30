using System;
using System.Collections.Generic;
using System.Linq;

namespace NgramAnalyzer.Common
{
    /// <inheritdoc cref="IEquatable&lt;NGram&gt;" />
    /// <summary>
    /// NGram structure which represents NGram model with value.
    /// </summary>
    public struct NGram : IEquatable<NGram>
    {
        #region FIELDS
        public readonly int Value;
        public readonly List<string> WordsList;

        public NGram(int value, List<string> wordsList)
        {
            Value = value;
            WordsList = wordsList;
        }

        #endregion

        #region OVERRIDES
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var result = WordsList.Aggregate("", (current, item) => current + (item + " "));
            result = Value + " " + result;
            result = result.Substring(0, result.Length - 1);
            result = result.Replace("{", "{{");
            result = result.Replace("}", "}}");

            return result;
        }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method changes special strings for database.
        /// </summary>
        public void ChangeSpecialCharacters()
        {
            for (var index = 0; index < WordsList.Count; ++index)
            {
                WordsList[index] = WordsList[index].ChangeSpecialCharacters();
            }
        }

        /// <summary>
        /// To the string array.
        /// </summary>
        /// <returns>String array.</returns>
        public List<string> ToStrings()
        {
            var result = new List<string>();
            foreach (var item in WordsList)
            {
                result.Add(item);
            }

            return result;
        }

        public bool Equals(NGram other)
        {
            if (Value != other.Value) return false;
            if (WordsList == null && other.WordsList == null) return true;
            if (WordsList == null || other.WordsList == null) return false;
            if (WordsList.Count != other.WordsList.Count) return false;
            return !WordsList.Where((t, i) => !t.Equals(other.WordsList[i])).Any();
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}

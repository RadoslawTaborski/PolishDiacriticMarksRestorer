using System.Collections.Generic;
using System.Linq;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// NGram structure which represents NGram model with value.
    /// </summary>
    public struct NGram
    {
        #region FIELDS
        public int Value;
        public List<string> WordsList;
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
                WordsList[index] = WordsList[index].Replace(@"\", @"\\");
                WordsList[index] = WordsList[index].Replace(@"'", @"\'");
            }
        }

        /// <summary>
        /// To the string array.
        /// </summary>
        /// <returns>String array.</returns>
        public string[] ToStrings()
        {
            var result = new List<string> {Value.ToString()};
            foreach (var item in WordsList)
            {
                result.Add(item);
            }

            return result.ToArray();
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}

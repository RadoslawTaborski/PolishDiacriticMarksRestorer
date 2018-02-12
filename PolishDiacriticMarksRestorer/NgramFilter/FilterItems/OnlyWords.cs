using System.Text.RegularExpressions;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    /// <summary>
    /// OnlyWords Class rejects ngram with strings which are only non-alphabetic marks
    /// </summary>
    internal class OnlyWords : IFilterItem
    {
        #region FIELDS

        #endregion

        #region CONSTRUCTORS

        #endregion

        #region  PUBLIC
        /// <inheritdoc />
        public bool IsCorrect(NGram ngram)
        {
            foreach (var item in ngram.WordsList)
            {
                if (OnlySpecialMarks(item)) return false;
            }

            return true;
        }
        #endregion

        #region PRIVATE
        private bool OnlySpecialMarks(string str)
        {
            var reg = new Regex(@"[^a-ząćęłńóśźżA-ZĄĆĘŁŃÓŚŹŻ]{" + str.Length + "}");
            var result = reg.IsMatch(str);
            return result;
        }
        #endregion
    }
}

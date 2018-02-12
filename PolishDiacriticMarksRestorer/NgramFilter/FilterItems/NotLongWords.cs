using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    /// <summary>
    /// NotLongWords Class rejects ngram with too long words
    /// </summary>
    internal class NotLongWords : IFilterItem
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
                if (item.Length > 25) return false;
            }

            return true;
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}

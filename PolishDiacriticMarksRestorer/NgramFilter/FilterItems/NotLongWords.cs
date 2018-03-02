using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    /// <summary>
    /// NotLongWords Class rejects ngram with too long words.
    /// </summary>
    /// <seealso cref="NgramFilter.Interfaces.IFilterItem" />
    internal class NotLongWords : IFilterItem
    {
        #region FIELDS

        #endregion

        #region CONSTRUCTORS

        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method validates the ngram.
        /// </summary>
        /// <param name="ngram">Ngram which is analyzed.</param>
        /// <returns>
        /// True if ngram is corrected.
        /// </returns>
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

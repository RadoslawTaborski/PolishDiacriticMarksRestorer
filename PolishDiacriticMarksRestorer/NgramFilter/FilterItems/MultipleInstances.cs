using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    /// <summary>
    /// MultipleInstances Class rejects ngram with value less than 2.
    /// </summary>
    /// <seealso cref="NgramFilter.Interfaces.IFilterItem" />
    internal class MultipleInstances : IFilterItem
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
            return ngram.Value > 1;
        }
        #endregion

        #region PRIVATE

        #endregion

    }
}

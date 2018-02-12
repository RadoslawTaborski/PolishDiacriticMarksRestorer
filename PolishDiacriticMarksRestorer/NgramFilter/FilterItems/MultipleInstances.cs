using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    /// <summary>
    /// MultipleInstances Class rejects ngram with value less than 2
    /// </summary>
    internal class MultipleInstances : IFilterItem
    {
        #region FIELDS

        #endregion

        #region CONSTRUCTORS

        #endregion

        #region  PUBLIC
        /// <inheritdoc cref="IFilterItem"/>
        public bool IsCorrect(NGram ngram)
        {
            return ngram.Value > 1;
        }
        #endregion

        #region PRIVATE

        #endregion

    }
}

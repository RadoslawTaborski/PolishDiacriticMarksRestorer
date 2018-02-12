using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    /// <summary>
    /// Filter Class stores and runs FilterItem.
    /// </summary>
    /// <seealso cref="NgramFilter.Interfaces.IFilter" />
    internal class Filter : IFilter
    {
        #region FIELDS
        private readonly List<IFilterItem> _filters;
        #endregion

        #region CONSTRUCTORS
        public Filter()
        {
            _filters = new List<IFilterItem>();
        }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// Add new FilterItem to list.
        /// </summary>
        /// <param name="item">FilterItem which will be added.</param>
        /// <inheritdoc />
        public void Add(IFilterItem item)
        {
            _filters.Add(item);
        }

        /// <summary>
        /// Size of FilterItem list.
        /// </summary>
        /// <returns>
        /// List size.
        /// </returns>
        /// <inheritdoc />
        public int Size()
        {
            return _filters.Count;
        }

        /// <summary>
        /// Run all added FilterItems and checks if the word meets the criteria.
        /// </summary>
        /// <param name="ngram">Ngram which is filtred.</param>
        /// <returns>
        /// True if ngram is corrected.
        /// </returns>
        /// <inheritdoc />
        public bool Start(NGram ngram)
        {
            foreach (var item in _filters)
            {
                if (!item.IsCorrect(ngram)) return false;
            }

            return true;
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}
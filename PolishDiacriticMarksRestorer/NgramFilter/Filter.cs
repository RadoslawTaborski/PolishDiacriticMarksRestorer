using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    /// <summary>
    /// Filter Class stores and runs FilterItem
    /// </summary>
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
        /// <inheritdoc />
        public void Add(IFilterItem item)
        {
            _filters.Add(item);
        }

        /// <inheritdoc />
        public int Size()
        {
            return _filters.Count;
        }

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
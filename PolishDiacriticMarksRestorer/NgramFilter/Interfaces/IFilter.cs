﻿using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    public interface IFilter
    {
        /// <summary>
        /// Run all added FilterItems and checks if the word meets the criteria.
        /// </summary>
        /// <param name="ngram">Ngram which is filtred.</param>
        /// <returns>
        /// True if ngram is corrected.
        /// </returns>
        bool Start(NGram ngram);
        /// <summary>
        /// Add new FilterItem to list.
        /// </summary>
        /// <param name="item">FilterItem which will be added.</param>
        void Add(IFilterItem item);
        /// <summary>
        /// Size of FilterItem list.
        /// </summary>
        /// <returns>
        /// List size.
        /// </returns>
        int Size();
    }
}

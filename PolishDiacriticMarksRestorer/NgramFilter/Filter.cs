﻿using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    internal class Filter : IFilter
    {
        private readonly List<IFilterItem> _filters;

        public Filter()
        {
            _filters = new List<IFilterItem>();
        }

        public void Add(IFilterItem item)
        {
            _filters.Add(item);
        }

        public int Size()
        {
            return _filters.Count;
        }

        public bool Start(NGram ngram)
        {
            foreach (var item in _filters)
            {
                if (!item.IsCorrect(ngram)) return false;
            }

            return true;
        }
    }
}
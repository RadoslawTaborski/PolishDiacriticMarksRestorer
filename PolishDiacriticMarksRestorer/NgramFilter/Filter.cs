using System.Collections.Generic;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    public class Filter : IFilter
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

        public bool Start(List<string> list)
        {
            foreach (var item in _filters)
            {
                if (!item.IsCorrect(list)) return false;
            }

            return true;
        }
    }
}
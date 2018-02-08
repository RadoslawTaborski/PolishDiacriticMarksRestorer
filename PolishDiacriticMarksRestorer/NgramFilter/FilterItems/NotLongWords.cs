using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    internal class NotLongWords : IFilterItem
    {
        public bool IsCorrect(NGram ngram)
        {
            foreach (var item in ngram.WordsList)
            {
                if (item.Length > 25) return false;
            }

            return true;
        }
    }
}

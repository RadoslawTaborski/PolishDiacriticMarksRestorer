using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    internal class MultipleInstances : IFilterItem
    {
        public bool IsCorrect(NGram ngram)
        {
            return ngram.Value > 1;
        }
    }
}

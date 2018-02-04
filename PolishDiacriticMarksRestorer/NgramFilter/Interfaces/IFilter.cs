using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    internal interface IFilter
    {
        bool Start(NGram ngram);
        void Add(IFilterItem item);
    }
}

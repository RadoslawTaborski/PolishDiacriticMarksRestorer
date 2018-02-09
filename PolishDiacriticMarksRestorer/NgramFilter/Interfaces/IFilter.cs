using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    public interface IFilter
    {
        bool Start(NGram ngram);
        void Add(IFilterItem item);
        int Size();
    }
}

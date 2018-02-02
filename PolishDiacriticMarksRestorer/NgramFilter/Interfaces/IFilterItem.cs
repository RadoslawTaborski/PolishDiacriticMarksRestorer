using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    public interface IFilterItem
    {
        bool IsCorrect(NGram ngram);
    }
}

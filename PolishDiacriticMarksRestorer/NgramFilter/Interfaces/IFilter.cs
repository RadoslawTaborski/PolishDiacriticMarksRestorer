using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    internal interface IFilter
    {
        bool Start(NGram ngram);
    }
}

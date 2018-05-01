using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface ISentenceSpliter
    {
        List<Sentence> Split(List<string> text);
    }
}

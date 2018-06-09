using System;
using System.Collections.Generic;
using System.Text;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface INgramsConnector
    {
        List<string> AnalyzeNgramsVariants(List<NGramVariants> ngramsVar, int length, int countWords);
    }
}

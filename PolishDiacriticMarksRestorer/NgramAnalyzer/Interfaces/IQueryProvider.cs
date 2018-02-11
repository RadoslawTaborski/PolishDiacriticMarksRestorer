using System;
using System.Collections.Generic;
using System.Text;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IQueryProvider
    {
        string GetNgramsFromTable(NgramType ngramType, List<string> wordList);
    }
}

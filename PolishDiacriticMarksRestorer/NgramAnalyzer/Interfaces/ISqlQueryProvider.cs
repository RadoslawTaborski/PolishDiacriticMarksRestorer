using System;
using System.Collections.Generic;
using System.Text;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface ISqlQueryProvider
    {
        string GetNgramsFromTable(NgramType ngramType, List<string> list);
    }
}

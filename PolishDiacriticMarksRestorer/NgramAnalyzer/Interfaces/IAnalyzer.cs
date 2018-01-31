using System;

namespace NgramAnalyzer.Interfaces
{
    interface IAnalyzer
    {
        String[] AnalyzeStrings(String[] str);
    }
}

using System;

namespace NgramAnalyzer.Interfaces
{
    internal interface IAnalyzer
    {
        string[] AnalyzeStrings(string[] str);
    }
}

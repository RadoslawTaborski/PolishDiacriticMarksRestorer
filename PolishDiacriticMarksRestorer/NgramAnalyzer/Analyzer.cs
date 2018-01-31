using System;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer
{
    public class Analyzer : IAnalyzer
    {
        private String[] _str;
        public string[] AnalyzeStrings(string[] str)
        {
            _str = str;
            return _str;
        }
    }
}

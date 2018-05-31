using System;
using System.Collections.Generic;
using System.Text;

namespace NgramAnalyzer.Common
{
    public class WordMap
    {
        public WordMap(string word)
        {
            Word = word;
        }

        public string Word { get; }

        public Dictionary<string, int> MappedWords { get; } = new Dictionary<string,int>();

        public void Add(string str, int val)
        {
            MappedWords.Add(str,val);
        }
    }
}

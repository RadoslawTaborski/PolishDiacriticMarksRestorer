using System.Text.RegularExpressions;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    internal class WordsWithoutNonPunctationMarks : IFilterItem
    {
        public bool IsCorrect(NGram ngram)
        {
            foreach (var item in ngram.WordsList)
            {
                if (NonPunctationMarks(item)) return false;
            }

            return true;
        }

        private bool NonPunctationMarks(string str)
        {
            var regex = new Regex(@"[^\,\;\!\?\(\)""\.\-\:a-ząćęłńóśźżA-ZĄĆĘŁŃÓŚŹŻ]");

            if (regex.IsMatch(str)) return true;

            regex = new Regex(@"[a-ząćęłńóśźżA-ZĄĆĘŁŃÓŚŹŻ]{" + str.Length + "}");

            if (regex.IsMatch(str)) return false;

            regex = new Regex(@"[\.]{4,}");

            if (regex.IsMatch(str)) return true;

            var copy = str;
            copy = Regex.Replace(copy, @"[\.]{3}", "");

            regex = new Regex(@"[\.]{2}");
            if (regex.IsMatch(copy)) return true;

            regex = new Regex(@"^[\(""]|[\;\,\?\!\-\:"")\.]$");

            while (regex.IsMatch(copy))
            {
                copy = Regex.Replace(copy, @"^[\(""]|[\;\,\?\!\-\:"")\.]$", "");
            }

            regex = new Regex(@"[\,\;\!\?\(\)""\.\:]");

            return regex.IsMatch(copy);
        }
    }
}

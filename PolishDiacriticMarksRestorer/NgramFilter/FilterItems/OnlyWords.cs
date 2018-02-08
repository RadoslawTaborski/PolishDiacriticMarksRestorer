using System.Text.RegularExpressions;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    internal class OnlyWords : IFilterItem
    {
        public bool IsCorrect(NGram ngram)
        {
            foreach (var item in ngram.WordsList)
            {
                if (OnlySpecialMarks(item)) return false;
            }

            return true;
        }

        private bool OnlySpecialMarks(string str)
        {
            var reg = new Regex(@"[^a-ząćęłńóśźżA-ZĄĆĘŁŃÓŚŹŻ]{" +str.Length + "}");
            var result = reg.IsMatch(str);
            return result;
        }
    }
}

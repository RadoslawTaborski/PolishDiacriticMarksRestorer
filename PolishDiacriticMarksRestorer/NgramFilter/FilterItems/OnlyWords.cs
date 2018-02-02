using System.Collections.Generic;
using System.Text.RegularExpressions;
using NgramFilter.Interfaces;

namespace NgramFilter.FilterItems
{
    internal class OnlyWords : IFilterItem
    {
        public bool IsCorrect(List<string> strList)
        {
            foreach (var item in strList)
            {
                if (OnlySpecialMarks(item)) return false;
            }

            return true;
        }

        private bool OnlySpecialMarks(string str)
        {
            var reg = new Regex(@"[^a-z]");
            var result = reg.IsMatch(str);
            return result;
        }
    }
}

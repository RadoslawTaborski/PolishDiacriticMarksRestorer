using System.Collections.Generic;
using System.Linq;

namespace NgramAnalyzer.Common
{
    public struct NGram
    {
        public int Value;
        public List<string> WordsList;

        public override string ToString()
        {
            var result = WordsList.Aggregate("", (current, item) => current + (item + " "));
            result = Value + " " + result;
            result = result.Replace("{", "{{");
            result = result.Replace("}", "}}");

            return result;
        }
    }
}

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

        public void ChangeSpecialCharacters()
        {
            for (var index = 0; index < WordsList.Count; ++index)
            {
                WordsList[index] = WordsList[index].Replace(@"\", @"\\");
                WordsList[index] = WordsList[index].Replace(@"'", @"\'");
            }
        }
    }
}

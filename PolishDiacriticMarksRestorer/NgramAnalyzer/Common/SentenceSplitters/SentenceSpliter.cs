using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common.SentenceSplitters
{
    public class SentenceSpliter : ISentenceSpliter
    {
        private readonly Regex _rgx = new Regex(@"[\.\?\!]");

        //private readonly List<string> _abbreviation = new List<string> {"itd."};
        public List<Sentence> Split(List<string> words)
        {
            var result = new List<Sentence>();

            var last = 0;
            for (var index = 0; index < words.Count; index++)
            {
                var word = words[index];

                if (!_rgx.IsMatch(word[word.Length - 1].ToString()) && index != words.Count - 1) continue;
                if (index + 1 < words.Count)
                    if (!char.IsUpper(words[index + 1][0])) continue;

                var count = 0;
                for (var i = word.Length - 1; i >= 0; --i)
                {
                    if (_rgx.IsMatch(word[i].ToString())) count++;
                }

                var str = new List<string>();
                for (var i = last; i < index; ++i)
                {
                    str.Add(words[i]);
                }

                var sign = words[index].GetLast(count);
                str.Add(count > 0 ? words[index].Remove(words[index].Length - count) : words[index]);

                result.Add(new Sentence(str, sign));

                last = index + 1;
            }

            return result;
        }
    }
}

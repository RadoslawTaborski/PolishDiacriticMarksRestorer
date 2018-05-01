using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NgramAnalyzer.Common
{
    public struct Sentence
    {
        public readonly List<string> Text;
        public readonly string EndMarks;

        public Sentence(List<string> text, string endMarks)
        {
            Text = text;
            EndMarks = endMarks;
        }

        public Sentence(Sentence sentence)
        {
            Text = new List<string>(sentence.Text);
            EndMarks = sentence.EndMarks;
        }

        public override string ToString()
        {
            var text = Text.Take(Text.Count - 1).Aggregate("", (current, word) => current + (word + " "));

            text += Text[Text.Count - 1] + EndMarks;

            return text;
        }
    }
}

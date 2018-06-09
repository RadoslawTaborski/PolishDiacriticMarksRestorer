using System;
using System.Collections.Generic;
using System.Text;

namespace NgramAnalyzer.Common.IInterpunctionManager
{
    public class InclusionManager : Interfaces.IInterpunctionManager
    {
        private readonly List<string> _inclusionSigns = new List<string>
        {
            "”",
            "„",
            "\"",
            "(",
            ")"
        };
        public NGram Remove(NGram actual)
        {
            var result = new NGram(actual);
            for (var i = 0; i < result.WordsList.Count; i++)
            {
                result.WordsList[i] = result.WordsList[i].WithoutPunctationMarks();
            }

            return result;
        }

        public NGram Restore(NGram old, NGram actual)
        {
            var ngram = new NGram(actual);
            for (var i = 0; i < old.WordsList.Count; i++)
            {
                var item = old.WordsList[i];
                var indexes = new List<int>();
                foreach (var sign in _inclusionSigns)
                {
                    indexes.AddRange(item.AllIndexesOf(sign));
                }
                indexes.Sort();

                foreach (var ind in indexes)
                {
                    ngram.WordsList[i] = ngram.WordsList[i].Insert(ind, old.WordsList[i][ind].ToString());
                }
            }

            return ngram;
        }
    }
}

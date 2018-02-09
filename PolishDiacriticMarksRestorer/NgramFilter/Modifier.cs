using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    internal class Modifier : IModifier
    {
        private readonly List<IModifierItem> _modifiers;

        public Modifier()
        {
            _modifiers = new List<IModifierItem>();
        }

        public void Add(IModifierItem item)
        {
            _modifiers.Add(item);
        }

        public int Size()
        {
            return _modifiers.Count;
        }

        public NGram Start(NGram ngram)
        {
            foreach (var item in _modifiers)
            {
                ngram = item.Edit(ngram);
            }

            return ngram;
        }
    }
}

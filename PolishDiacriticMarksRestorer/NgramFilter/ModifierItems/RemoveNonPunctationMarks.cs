using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.ModifierItems
{
    internal class RemoveNonPunctationMarks: IModifierItem
    {
        public NGram Edit(NGram ngram)
        {
            return ngram;
        }
    }
}

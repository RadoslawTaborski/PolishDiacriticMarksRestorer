using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    public interface IModifierItem
    {
        NGram Edit(NGram ngram);
    }
}

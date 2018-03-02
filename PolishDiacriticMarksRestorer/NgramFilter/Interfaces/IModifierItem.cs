using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    public interface IModifierItem
    {
        /// <summary>
        /// This method edit ngram.
        /// </summary>
        /// <param name="ngram">Ngram which will be edited.</param>
        /// <returns>
        /// Edited ngram.
        /// </returns>
        NGram Edit(NGram ngram);
    }
}

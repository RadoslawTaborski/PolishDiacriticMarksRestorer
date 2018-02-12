using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    public interface IFilterItem
    {
        /// <summary>
        /// This method validates the ngram.
        /// </summary>
        /// <param name="ngram">Ngram which is analyzed.</param>
        /// <returns>
        /// True if ngram is corrected.
        /// </returns>
        bool IsCorrect(NGram ngram);
    }
}

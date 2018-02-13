using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IQueryProvider
    {
        /// <summary>
        /// Generate query to get the same ngrams from Ngrams table.
        /// </summary>
        /// <param name="ngramType">Type of NGrams.</param>
        /// <param name="wordList">List of words - must have a size suitable for the type ngrams.</param>
        /// <returns>
        /// Query string.
        /// </returns>
        string GetTheSameNgramsFromTable(NgramType ngramType, List<string> wordList);

        /// <summary>
        /// Generate query to get the similar ngrams from Ngrams table.
        /// </summary>
        /// <param name="ngramType">Type of the ngram.</param>
        /// <param name="wordList">List of words - must have a one size smaller than the type ngrams.</param>
        /// <returns></returns>
        string GetSimilarNgramsFromTable(NgramType ngramType, List<string> wordList);
    }
}

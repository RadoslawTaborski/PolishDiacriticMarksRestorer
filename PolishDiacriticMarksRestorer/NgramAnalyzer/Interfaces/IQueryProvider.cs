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
        /// Generate query which gets the ngrams changing the last word from table.
        /// </summary>
        /// <param name="ngramType">Type of the ngram.</param>
        /// <param name="wordList">The word list.</param>
        /// <param name="combinations">Possible last words.</param>
        /// <returns></returns>
        string GetMultiNgramsFromTable(NgramType ngramType, List<string> wordList, List<string> combinations);
        /// <summary>
        /// Generate query to get the similar ngrams from Ngrams table.
        /// </summary>
        /// <param name="ngramType">Type of the ngram.</param>
        /// <param name="wordList">List of words - must have a one size smaller than the type ngrams.</param>
        /// <returns>
        /// Query string.
        /// </returns>
        string GetSimilarNgramsFromTable(NgramType ngramType, List<string> wordList);
        /// <summary>
        /// Generate query which checks the words in unigram table.
        /// </summary>
        /// <param name="wordList">The word list.</param>
        /// <returns>
        /// Query string.
        /// </returns>
        string CheckWordsInUnigramFromTable(List<string> wordList);
        /// <summary>
        /// Gets all necessary ngrams from table. Each list in outside list is responsible for one type of ngrams.
        /// Inside list contains available word combinations.
        /// </summary>
        /// <param name="ngramType">Type of the ngram.</param>
        /// <param name="wordLists">The word lists.</param>
        /// <returns>
        /// Query string.
        /// </returns>
        string GetAllNecessaryNgramsFromTable(NgramType ngramType, List<List<List<string>>> wordLists);
    }
}

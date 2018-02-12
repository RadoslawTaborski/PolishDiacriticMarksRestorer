using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IQueryProvider
    {
        /// <summary>
        /// Generate query to get data from Ngrams table
        /// </summary>
        /// <param name="ngramType">type of NGrams</param>
        /// <param name="wordList">list of words - must have a size suitable for the type ngrams</param>
        /// <returns>return query string</returns>
        string GetNgramsFromTable(NgramType ngramType, List<string> wordList);
    }
}

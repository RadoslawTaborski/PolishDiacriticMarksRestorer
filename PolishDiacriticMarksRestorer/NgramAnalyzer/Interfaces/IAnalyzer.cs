using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    internal interface IAnalyzer
    {
        /// <summary>
        /// This method analyze correctness input.
        /// </summary>
        /// <param name="strArray">array of strings to analyze.</param>
        /// <returns>
        /// string array with result of analyze.
        /// </returns>
        List<string> AnalyzeString(string text);
        /// <summary>
        /// This method set a IQueryProvider.
        /// </summary>
        /// <param name="queryProvider">The query provider.</param>
        void SetQueryProvider(IQueryProvider queryProvider);
        /// <summary>
        /// This method set a NgramType.
        /// </summary>
        /// <param name="type">The type.</param>
        void SetNgram(NgramType type);
        /// <summary>
        /// This method set a IDataAccess.
        /// </summary>
        /// <param name="db">The database.</param>
        void SetData(IDataAccess db);
        int SetWords(string text);
        List<List<string>> CreateWordsCombinations();
    }
}

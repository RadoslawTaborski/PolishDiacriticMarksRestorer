using System;
using System.Collections.Generic;
using Google.Protobuf;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IAnalyzer
    {
        List<string> Output { get; }
        List<string> Input { get; }
        List<string> InputWithWhiteMarks { get; }

        /// <summary>
        /// This method analyze correctness input.
        /// </summary>
        /// <param name="text">text to analyze.</param>
        /// <param name="times"></param>
        /// <param name="counts"></param>
        /// <returns>
        /// strings list with result of analyze together with white marks.
        /// </returns>
        List<string> AnalyzeString(string text, out List<TimeSpan> times, out List<int> counts);
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
        List<Tuple<List<string>, bool>> CreateWordsCombinations();
    }
}

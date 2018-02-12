using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    internal interface IAnalyzer
    {
        /// <summary>
        /// This method analyze correctness input
        /// </summary>
        /// <param name="strArray">array of strings to analyze</param>
        /// <returns>string array with result of analyze</returns>
        string[] AnalyzeStrings(string[] strArray);
        /// <summary>
        /// This method set a IQueryProvider
        /// </summary>
        /// <param name="queryProvider"></param>
        void SetQueryProvider(IQueryProvider queryProvider);
        /// <summary>
        /// This method set a NgramType
        /// </summary>
        /// <param name="type"></param>
        void SetNgram(NgramType type);
        /// <summary>
        /// This method set a IDataAccess
        /// </summary>
        /// <param name="db"></param>
        void SetData(IDataAccess db);
    }
}

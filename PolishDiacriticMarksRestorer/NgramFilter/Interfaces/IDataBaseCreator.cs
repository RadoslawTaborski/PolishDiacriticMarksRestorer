using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    internal interface IDataBaseCreator
    {
        void CreateDataBase(string name);
        void CreateTable(string dataBaseName, string tableName, int numberOfWords);
        void AddNgramToTable(string tableName, NGram ngram);
        void OpenDb();
        void CloseDb();
    }
}

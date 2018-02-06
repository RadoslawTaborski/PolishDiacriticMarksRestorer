using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    internal interface IDataBaseCreator
    {
        void CreateDataBase(string name);
        void CreateTable(string dataBaseName, string tableName, int numberOfWords);
        void AddNgramsToTable(string tableName, List<NGram> ngrams);
        void OpenDb();
        void CloseDb();
    }
}

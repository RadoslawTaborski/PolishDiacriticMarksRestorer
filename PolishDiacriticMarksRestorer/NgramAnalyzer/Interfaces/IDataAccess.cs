using System.Data;

namespace NgramAnalyzer.Interfaces
{
    public interface IDataAccess
    {
        bool ConnectToDb();
        bool ConnectToServer();
        bool Disconnect();
        DataSet ExecuteSqlCommand(string query);
        void ExecuteNonQueryServer(string query);
        void ExecuteNonQueryDb(string query);
    }
}

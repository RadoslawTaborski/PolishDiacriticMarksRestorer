using System.Data;

namespace NgramAnalyzer.Interfaces
{
    public interface IDataAccess
    {
        void ConnectToDb();
        void ConnectToServer();
        void Disconnect();
        DataSet ExecuteSqlCommand(string query);
        void ExecuteNonQueryServer(string query);
        void ExecuteNonQueryDb(string query);
    }
}

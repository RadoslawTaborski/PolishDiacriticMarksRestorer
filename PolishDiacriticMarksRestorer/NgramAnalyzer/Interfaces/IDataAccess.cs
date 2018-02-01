using System.Data;

namespace NgramAnalyzer.Interfaces
{
    public interface IDataAccess
    {
        bool Connect();
        bool Disconnect();
        DataSet ExecuteSqlCommand(string query);
    }
}

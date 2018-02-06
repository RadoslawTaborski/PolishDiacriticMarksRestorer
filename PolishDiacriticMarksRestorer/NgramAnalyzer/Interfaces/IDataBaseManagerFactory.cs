using System.Data;

namespace NgramAnalyzer.Interfaces
{
    public interface IDataBaseManagerFactory
    {
        IDbConnection CreateConnectionDb(string connectionString);
        IDbConnection CreateConnectionServer(string connectionString);
        IDataAdapter CreateDataAdapter(string query);
    }
}

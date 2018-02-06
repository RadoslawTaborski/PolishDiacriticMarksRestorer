using System.Data;
using MySql.Data.MySqlClient;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    public class MySqlConnectionFactory : IDataBaseManagerFactory
    {
        private MySqlConnection _dbConnection;
        private MySqlConnection _serverDbConnection;

        public IDbConnection CreateConnectionDb(string connectionString)
        {
            _dbConnection= new MySqlConnection(connectionString);
            return _dbConnection;
        }

        public IDbConnection CreateConnectionServer(string connectionString)
        {
            _serverDbConnection = new MySqlConnection(connectionString);
            return _serverDbConnection;
        }

        public IDataAdapter CreateDataAdapter(string query)
        {
            return new MySqlDataAdapter(query, _dbConnection);
        }
    }
}

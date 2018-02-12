using System.Data;
using MySql.Data.MySqlClient;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// MySqlConnectionFactory Class create MySql instances of connections
    /// </summary>
    public class MySqlConnectionFactory : IDataBaseManagerFactory
    {
        #region FIELDS
        private MySqlConnection _dbConnection;
        private MySqlConnection _serverDbConnection;
        #endregion

        #region CONSTRUCTORS

        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method create connection to MySql database
        /// </summary>
        /// <param name="connectionString">MySql command query</param>
        /// <returns>MySqlConnection to database</returns>
        public IDbConnection CreateConnectionDb(string connectionString)
        {
            _dbConnection = new MySqlConnection(connectionString);
            return _dbConnection;
        }

        /// <summary>
        /// This method create connection to MySql server
        /// </summary>
        /// <param name="connectionString">MySql command query</param>
        /// <returns>MySqlConnection to database server</returns>
        public IDbConnection CreateConnectionServer(string connectionString)
        {
            _serverDbConnection = new MySqlConnection(connectionString);
            return _serverDbConnection;
        }

        /// <summary>
        /// This method create MySql Data Adapter
        /// </summary>
        /// <param name="query">MySql command query</param>
        /// <returns>MySqlDataAdapter to database</returns>
        public IDataAdapter CreateDataAdapter(string query)
        {
            //var cmd = new MySqlCommand(query, _dbConnection)
            return new MySqlDataAdapter(query, _dbConnection);
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}

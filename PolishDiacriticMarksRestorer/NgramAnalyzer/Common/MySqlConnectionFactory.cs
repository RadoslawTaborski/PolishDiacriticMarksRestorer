using System.Data;
using MySql.Data.MySqlClient;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// MySqlConnectionFactory Class create MySql instances of connections.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IDataBaseManagerFactory" />
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
        /// This method create connection to MySql database.
        /// </summary>
        /// <param name="connectionString">MySql command query.</param>
        /// <returns>
        /// MySqlConnection to database.
        /// </returns>
        /// <inheritdoc />
        public IDbConnection CreateConnectionDb(string connectionString)
        {
            _dbConnection = new MySqlConnection(connectionString);
            return _dbConnection;
        }

        /// <summary>
        /// This method create connection to MySql server.
        /// </summary>
        /// <param name="connectionString">MySql command query.</param>
        /// <returns>
        /// MySqlConnection to database server.
        /// </returns>
        /// <inheritdoc />
        public IDbConnection CreateConnectionServer(string connectionString)
        {
            _serverDbConnection = new MySqlConnection(connectionString);
            return _serverDbConnection;
        }

        /// <summary>
        /// This method create MySql Data Adapter.
        /// </summary>
        /// <param name="query">MySql command query.</param>
        /// <returns>
        /// MySqlDataAdapter to database.
        /// </returns>
        public IDataAdapter CreateDataAdapter(string query)
        {
            var da = new MySqlDataAdapter(query, _dbConnection) {SelectCommand = {CommandTimeout = 2147483 } };
            return da;
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}

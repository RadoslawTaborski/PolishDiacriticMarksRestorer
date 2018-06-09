using System.Collections.Generic;
using System.Data;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// DataBaseManager Class allows to connect to database.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IDataAccess" />
    public class DataBaseManager : IDataAccess
    {
        #region FIELDS
        private readonly IDataBaseManagerFactory _connectionFactory;
        private IDbConnection _connectionDb;
        private IDbConnection _connectionServer;
        private readonly string _server;
        private readonly string _database;
        private readonly string _uid;
        private readonly string _password;
        private Dictionary<string, DataSet> _cache;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="DataBaseManager"/> class.
        /// </summary>
        /// <param name="dbFactory">The database factory.</param>
        /// <param name="server">The server address.</param>
        /// <param name="database">The database name.</param>
        /// <param name="uid">The user name.</param>
        /// <param name="password">The password.</param>
        public DataBaseManager(IDataBaseManagerFactory dbFactory, string server, string database, string uid, string password)
        {
            _server = server;
            _database = database;
            _uid = uid;
            _password = password;
            _connectionFactory = dbFactory;
            _cache = new Dictionary<string, DataSet>();
        }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method connects with database.
        /// </summary>
        /// <inheritdoc />
        public void ConnectToDb()
        {
            _connectionDb = _connectionFactory.CreateConnectionDb(InitializeDbString());
            _connectionDb.Open();
        }

        public bool DbIsOpen()
        {
            return _connectionDb.State == ConnectionState.Open;
        }

        /// <summary>
        /// This method connects to server with databases.
        /// </summary>
        /// <inheritdoc />
        public void ConnectToServer()
        {
            _connectionServer = _connectionFactory.CreateConnectionServer(InitializeServerString());
            _connectionServer.Open();
        }

        /// <summary>
        /// This method disconnects database and server.
        /// </summary>
        /// <inheritdoc />
        public void Disconnect()
        {
            _connectionDb?.Close();
            _connectionServer?.Close();
        }

        /// <summary>
        /// This method performs a query to the database waiting for the result.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>
        /// DataSet which has data from database.
        /// </returns>
        /// <inheritdoc />
        public DataSet ExecuteSqlCommand(string query)
        {
            var ds = new DataSet();

            if (_cache.ContainsKey(query))
                ds = _cache[query];
            else
            {
                var adp = _connectionFactory.CreateDataAdapter(query);
                adp.Fill(ds);
                _cache.Add(query,ds);
            }

            return ds;
        }

        /// <summary>
        /// This method performs a command to the server.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <inheritdoc />
        public void ExecuteNonQueryServer(string query)
        {
            var command = _connectionServer.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            command.CommandTimeout = 2147483;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// This method performs a command to the database.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <inheritdoc />
        public void ExecuteNonQueryDb(string query)
        {
            var command = _connectionDb.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// This method disconnects database and server.
        /// </summary>
        /// <inheritdoc />
        public void Dispose()
        {
            Disconnect();
        }
        #endregion

        #region PRIVATE
        private string InitializeDbString()
        {
            return "SERVER=" + _server + ";" +
                   "DATABASE=" + _database + ";" +
                   "UID=" + _uid + ";" +
                   "PASSWORD=" + _password + ";" +
                   "SslMode=none; charset=utf8;" +
                   "Allow User Variables=True;";
        }

        private string InitializeServerString()
        {
            return $@"Server={_server};
                User ID={_uid};
                Password={_password};
                Pooling=false;
                SslMode = none;
                charset=utf8;
                Allow User Variables=True";
        }
        #endregion
    }
}

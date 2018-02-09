using System.Data;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    public class DataBaseManager : IDataAccess
    {
        private readonly IDataBaseManagerFactory _connectionFactory;
        private IDbConnection _connectionDb;
        private IDbConnection _connectionServer;
        private readonly string _server;
        private readonly string _database;
        private readonly string _uid;
        private readonly string _password;

        public DataBaseManager(IDataBaseManagerFactory dbFactory, string server, string database, string uid, string password)
        {
            _server = server;
            _database = database;
            _uid = uid;
            _password = password;
            _connectionFactory = dbFactory;
            
        }

        public void ConnectToDb()
        {
            _connectionDb = _connectionFactory.CreateConnectionDb(InitializeDbString());
            _connectionDb.Open();
        }

        public void ConnectToServer()
        {
            _connectionServer = _connectionFactory.CreateConnectionServer(InitializeServerString());
            _connectionServer.Open();
        }

        public void Disconnect()
        {
            _connectionDb?.Close();
            _connectionServer?.Close();
        }

        public DataSet ExecuteSqlCommand(string query)
        {
            var ds = new DataSet();
            var adp = _connectionFactory.CreateDataAdapter(query);
            adp.Fill(ds);

            return ds;
        }

        public void ExecuteNonQueryServer(string query)
        {
            var command = _connectionServer.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        public void ExecuteNonQueryDb(string query)
        {
            var command = _connectionDb.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        private string InitializeDbString()
        {
            return "SERVER=" + _server + ";" +
                "DATABASE=" + _database + ";" +
                "UID=" + _uid + ";" +
                "PASSWORD=" + _password + ";" +
                "SslMode=none; charset=utf8;" +
                "Allow User Variables=True";
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

        public void Dispose()
        {
            Disconnect();
        }
    }
}

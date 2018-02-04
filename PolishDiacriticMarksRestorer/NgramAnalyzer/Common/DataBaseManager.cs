using System.Data;
using MySql.Data.MySqlClient;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    public class DataBaseManager : IDataAccess
    {
        private MySqlConnection _connectionDb;
        private MySqlConnection _connectionServer;
        private readonly string _server;
        private readonly string _database;
        private readonly string _uid;
        private readonly string _password;

        public DataBaseManager(string server, string database, string uid, string password)
        {
            _server = server;
            _database = database;
            _uid = uid;
            _password = password;
            Initialize();
        }

        public bool ConnectToDb()
        {
            _connectionDb.Open();
            return true;
        }

        public bool ConnectToServer()
        {
            var connection = $@"Server={_server};
                User ID={_uid};
                Password={_password};
                Pooling=false;
                SslMode = none;
                charset=utf8";
            _connectionServer = new MySqlConnection(connection);

            _connectionServer.Open();
            return true;
        }

        public bool Disconnect()
        {
            _connectionDb?.Close();
            _connectionServer?.Close();
            return true;
        }

        public DataSet ExecuteSqlCommand(string query)
        {
            var ds = new DataSet();

            var response = new MySqlCommand(query, _connectionDb);
            var adp = new MySqlDataAdapter(response) { SelectCommand = response };
            adp.Fill(ds);

            return ds;
        }

        public void ExecuteNonQueryServer(string query)
        {
            var response = new MySqlCommand(query, _connectionServer);
            response.ExecuteNonQuery();
        }

        public void ExecuteNonQueryDb(string query)
        {
            var response = new MySqlCommand(query, _connectionDb);
            response.ExecuteNonQuery();
        }


        private void Initialize()
        {
            var connectionString = "SERVER=" + _server + ";" + "DATABASE=" +
                                      _database + ";" + "UID=" + _uid + ";" + "PASSWORD=" + _password + ";" + "SslMode=none; charset=utf8";

            _connectionDb = new MySqlConnection(connectionString);
        }
    }
}

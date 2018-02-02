using System.Data;
using MySql.Data.MySqlClient;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    public class DataBaseManager : IDataAccess
    {
        private MySqlConnection _connection;
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

        public bool Connect()
        {
            _connection.Open();
            return true;
        }

        public bool Disconnect()
        {
            _connection.Close();
            return true;
        }

        public DataSet ExecuteSqlCommand(string query)
        {
            var ds = new DataSet();
            if (!Connect()) return null;

            var response = new MySqlCommand(query, _connection);
            var adp = new MySqlDataAdapter(response) { SelectCommand = response };
            adp.Fill(ds);
            Disconnect();

            return ds;
        }

        private void Initialize()
        {
            var connectionString = "SERVER=" + _server + ";" + "DATABASE=" +
                                      _database + ";" + "UID=" + _uid + ";" + "PASSWORD=" + _password + ";" + "SslMode=none";

            _connection = new MySqlConnection(connectionString);
        }
    }
}

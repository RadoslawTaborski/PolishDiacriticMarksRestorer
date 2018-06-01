using System;
using System.Data;

namespace NgramAnalyzer.Interfaces
{
    public interface IDataAccess : IDisposable
    {
        /// <summary>
        /// This method connects with database.
        /// </summary>
        void ConnectToDb();

        bool DbIsOpen();
        /// <summary>
        /// This method connects to server with databases.
        /// </summary>
        void ConnectToServer();
        /// <summary>
        /// This method disconnects database and server.
        /// </summary>
        void Disconnect();
        /// <summary>
        /// This method performs a query to the database waiting for the result.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>
        /// DataSet witch has data from database.
        /// </returns>
        DataSet ExecuteSqlCommand(string query);
        /// <summary>
        /// This method performs a command to the server.
        /// </summary>
        /// <param name="query">The query.</param>
        void ExecuteNonQueryServer(string query);
        /// <summary>
        /// This method performs a command to the database.
        /// </summary>
        /// <param name="query">The query.</param>
        void ExecuteNonQueryDb(string query);
    }
}

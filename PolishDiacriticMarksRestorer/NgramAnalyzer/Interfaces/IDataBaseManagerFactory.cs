using System.Data;

namespace NgramAnalyzer.Interfaces
{
    public interface IDataBaseManagerFactory
    {
        /// <summary>
        /// Creates the connection database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        IDbConnection CreateConnectionDb(string connectionString);
        /// <summary>
        /// Creates the connection server.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        IDbConnection CreateConnectionServer(string connectionString);
        /// <summary>
        /// Creates the data adapter.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        IDataAdapter CreateDataAdapter(string query);
    }
}

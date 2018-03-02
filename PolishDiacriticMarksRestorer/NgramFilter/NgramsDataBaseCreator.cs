using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    /// <summary>
    /// NgramsDataBaseCreator Class creates DataBase and Table and fill them.
    /// </summary>
    /// <seealso cref="NgramFilter.Interfaces.IDataBaseCreator" />
    internal class NgramsDataBaseCreator : IDataBaseCreator
    {
        #region FIELDS
        private readonly IDataAccess _db;
        private readonly IQueryProvider _provider;
        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the <see cref="NgramsDataBaseCreator"/> class.
        /// </summary>
        /// <param name="db">The data access class.</param>
        /// <param name="provider">Query to Sql database provider.</param>
        public NgramsDataBaseCreator(IDataAccess db, IQueryProvider provider)
        {
            _db = db;
            _provider = provider;
        }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method create new database on server.
        /// </summary>
        /// <param name="name">Name of database.</param>
        /// <inheritdoc />
        public void CreateDataBase(string name)
        {
            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(_provider.CreateDbString(name));
            _db.Disconnect();
        }

        /// <summary>
        /// This method create new table in database.
        /// </summary>
        /// <param name="dataBaseName">Name of database.</param>
        /// <param name="tableName">Name of table.</param>
        /// <param name="numberOfWords">Number of column with words.</param>
        /// <inheritdoc />
        public void CreateTables(string dataBaseName, string tableName, int numberOfWords)
        {
            if (numberOfWords < 1) return;

            var commandText = _provider.CreateNgramsTableString(dataBaseName, tableName, numberOfWords);
            var commandText1 = _provider.CreateAddProcedureString(dataBaseName, tableName, numberOfWords);

            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(commandText);
            _db.ExecuteNonQueryServer(commandText1);
            _db.Disconnect();
        }

        /// <summary>
        /// This method add Ngrams data to table.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="ngrams">List of ngrams.</param>
        /// <inheritdoc />
        public void AddNgramsToTable(string tableName, List<NGram> ngrams)
        {
            if (ngrams == null || ngrams.Count <= 0) return;

            var commandText = _provider.InsertNgramsString(tableName, ngrams);

            _db.ConnectToDb();
            _db.ExecuteNonQueryDb(commandText);
            _db.Disconnect();
        }

        /// <summary>
        /// This method add news ngrams or update if already exists the same ngram in database.
        /// </summary>
        /// <param name="tableName">Name of table.</param>
        /// <param name="ngrams">List of ngrams.</param>
        /// <inheritdoc />
        public void AddOrUpdateNgramsToTable(string tableName, List<NGram> ngrams)
        {
            if (ngrams == null || ngrams.Count <= 0) return;

            _db.ConnectToDb();
            foreach (var item in ngrams)
            {
                var commandText = _provider.InsertOrUpdateNgramString(item);
                _db.ExecuteNonQueryDb(commandText);
            }
            _db.Disconnect();
        }
        #endregion

        #region PRIVATE
        #endregion
    }
}

using System.Linq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer
{
    /// <summary>
    /// Analyze class compare data form database and input and provides result of Analyze
    /// </summary>
    public class Analyzer : IAnalyzer
    {
        #region FIELDS
        private IDataAccess _db;
        private IQueryProvider _queryProvider;
        private NgramType _ngramType;
        #endregion

        #region CONSTRUCTORS

        #endregion

        #region  PUBLIC
        /// <inheritdoc />
        public void SetData(IDataAccess db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public void SetNgram(NgramType type)
        {
            _ngramType = type;
        }

        /// <inheritdoc />
        public void SetQueryProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        /// <inheritdoc />
        public string[] AnalyzeStrings(string[] strArray)
        {
            return GetData(strArray);
        }
        #endregion

        #region PRIVATE
        private string[] GetData(string[] str)
        {
            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.GetNgramsFromTable(_ngramType, str.ToList()));
            _db.Disconnect();
            var dataRow = data.Tables[0].Rows[0].ItemArray;

            return dataRow.Select(item => item.ToString()).ToArray();
        }
        #endregion
    }
}

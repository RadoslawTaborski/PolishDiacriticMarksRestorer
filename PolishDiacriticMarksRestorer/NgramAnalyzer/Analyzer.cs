using System.Linq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer
{
    public class Analyzer : IAnalyzer
    {
        private IDataAccess _db;
        private IQueryProvider _queryProvider;
        private NgramType _ngramType;

        public void SetData(IDataAccess db)
        {
            _db = db;
        }

        public void SetNgram(NgramType type)
        {
            _ngramType = type;
        }

        public void SetQueryProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public string[] AnalyzeStrings(string[] str)
        {
            return GetData(str);
        }

        private string[] GetData(string[] str)
        {
            _db.ConnectToDb();
            var data = _db.ExecuteSqlCommand(_queryProvider.GetNgramsFromTable(_ngramType, str.ToList()));
            _db.Disconnect();
            var dataRow = data.Tables[0].Rows[0].ItemArray;

            return dataRow.Select(item => item.ToString()).ToArray();
        }
    }
}

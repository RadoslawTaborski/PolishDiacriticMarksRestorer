using System.Linq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer
{
    public class Analyzer : IAnalyzer
    {
        private IDataAccess _db;
        private NgramType _type;

        public void SetData(IDataAccess db, NgramType type)
        {
            _db = db;
            _type = type;
        }

        public string[] AnalyzeStrings(string[] str)
        {
            return GetData();
        }

        private string[] GetData()
        {
            var data = _db.ExecuteSqlCommand("SELECT * FROM `dane`");
            var dataRow = data.Tables[0].Rows[0].ItemArray;

            return dataRow.Select(item => item.ToString()).ToArray();
        }
    }
}

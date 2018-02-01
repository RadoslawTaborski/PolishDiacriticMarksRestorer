using System.Collections.Generic;
using System.Linq;
using NgramAnalyzer.Interfaces;
using NgramAnalyzer.Common;

namespace NgramAnalyzer
{
    public class Analyzer : IAnalyzer
    {
        private IDataAccess _db;
        public Analyzer(string server, string database, string uid, string password)
        {
            _db = new DataBase(server, database, uid, password);
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

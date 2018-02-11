using System;
using System.Collections.Generic;
using NgramAnalyzer.Common;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer
{
    public class SqlQueryProvider : IQueryProvider
    {
        private readonly IList<string> _dbTableDbTableName;

        public SqlQueryProvider(IList<string> dbTableNames)
        {
            if (dbTableNames == null || dbTableNames.Count != 4)
                throw new ArgumentException("dbTableNames IList has wrong size");
            _dbTableDbTableName = dbTableNames;
        }

        public string GetNgramsFromTable(NgramType ngramType, List<string> list)
        {
            var number = (int)ngramType;

            var query = "SELECT * FROM " + _dbTableDbTableName[number-1] +" WHERE ";

            for(var i =0 ; i < number; ++i)
            {
                if (i != 0) query += "AND ";
                query += "Word" +(i+1)+ "='"+list[i]+"' ";
            }

            var strBuilder =
                new System.Text.StringBuilder(query) { [query.Length - 1] = ';' };
            query = strBuilder.ToString();

            return query;
        }
    }
}

using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer
{
    public class SqlQueryProvider : ISqlQueryProvider
    {
        public string GetNgramsFromTable(NgramType ngramType, List<string> list)
        {
            var number = (int)ngramType;
            var dbTableName = new[]
            {
                "1grams",
                "2grams",
                "3grams",
                "4grams"
            };

            var query = "SELECT * FROM " + dbTableName[number-1] +" WHERE ";

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

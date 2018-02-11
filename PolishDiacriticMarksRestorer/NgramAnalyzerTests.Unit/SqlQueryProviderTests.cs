using System.Collections.Generic;
using NgramAnalyzer;
using NgramAnalyzer.Common;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class SqlQueryProviderTests
    {
        [Fact]
        public void GetNgramsFromTable_Unigrams()
        {
            var list = new List<string>
            {
                "a"
            };
            var provider = new SqlQueryProvider();
            var result = provider.GetNgramsFromTable(NgramType.Unigram, list);

            const string str = "SELECT * FROM 1grams WHERE Word1='a';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetNgramsFromTable_Digrams()
        {
            var list = new List<string>
            {
                "a",
                "b"
            };
            var provider = new SqlQueryProvider();
            var result = provider.GetNgramsFromTable(NgramType.Digram, list);

            const string str = "SELECT * FROM 2grams WHERE Word1='a' AND Word2='b';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetNgramsFromTable_Trisgrams()
        {
            var list = new List<string>
            {
                "a",
                "b",
                "c"
            };
            var provider = new SqlQueryProvider();
            var result = provider.GetNgramsFromTable(NgramType.Trigram, list);

            const string str = "SELECT * FROM 3grams WHERE Word1='a' AND Word2='b' AND Word3='c';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetNgramsFromTable_Fourgrams()
        {
            var list = new List<string>
            {
                "a",
                "b",
                "c",
                "d"
            };
            var provider = new SqlQueryProvider();
            var result = provider.GetNgramsFromTable(NgramType.Fourgram, list);

            const string str = "SELECT * FROM 4grams WHERE Word1='a' AND Word2='b' AND Word3='c' AND Word4='d';";
            Assert.Equal(str, result);
        }
    }
}

using System;
using System.Collections.Generic;
using NgramAnalyzer;
using NgramAnalyzer.Common;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class SqlQueryProviderTests
    {
        private readonly List<string> _names = new List<string>
        {
            "uni",
            "di",
            "tri",
            "four"
        };

        [Fact]
        public void SqlQueryProvider_NullListWithNames()
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new SqlQueryProvider(null));
            Assert.Equal("IList<string> 'dbTableNames' has wrong size", ex.Message);
        }

        [Fact]
        public void SqlQueryProvider_WrongListSize()
        {
        var list = new List<string>
        {
            "uni",
            "di",
            "tri",
        };
        Exception ex = Assert.Throws<ArgumentException>(() => new SqlQueryProvider(list));
            Assert.Equal("IList<string> 'dbTableNames' has wrong size", ex.Message);
        }

        [Fact]
        public void GetNgramsFromTable_NullListWithNames()
        {
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetNgramsFromTable(NgramType.Digram, null));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetNgramsFromTable_WrongListSize()
        {
            var list = new List<string>
            {
                "a"
            };

            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetNgramsFromTable(NgramType.Digram, list));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetNgramsFromTable_Unigrams()
        {
            var list = new List<string>
            {
                "a"
            };

            var provider = new SqlQueryProvider(_names);
            var result = provider.GetNgramsFromTable(NgramType.Unigram, list);

            const string str = "SELECT * FROM uni WHERE Word1='a';";
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

            var provider = new SqlQueryProvider(_names);
            var result = provider.GetNgramsFromTable(NgramType.Digram, list);

            const string str = "SELECT * FROM di WHERE Word1='a' AND Word2='b';";
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

            var provider = new SqlQueryProvider(_names);
            var result = provider.GetNgramsFromTable(NgramType.Trigram, list);

            const string str = "SELECT * FROM tri WHERE Word1='a' AND Word2='b' AND Word3='c';";
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

            var provider = new SqlQueryProvider(_names);
            var result = provider.GetNgramsFromTable(NgramType.Fourgram, list);

            const string str = "SELECT * FROM four WHERE Word1='a' AND Word2='b' AND Word3='c' AND Word4='d';";
            Assert.Equal(str, result);
        }
    }
}

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

        private readonly List<string> _wordList = new List<string>
        {
            "a",
            "b",
            "c",
            "d"
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
        public void GetTheSameNgramsFromTable_NullListWithNames()
        {
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetTheSameNgramsFromTable(NgramType.Digram, null));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_WrongListSize()
        {
            var list = new List<string>
            {
            "a",
            };

            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetTheSameNgramsFromTable(NgramType.Digram, list));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Unigrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Unigram, _wordList);

            const string str = "SELECT * FROM uni WHERE Word1='a';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Digrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Digram, _wordList);

            const string str = "SELECT * FROM di WHERE Word1='a' AND Word2='b';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Trigrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Trigram, _wordList);

            const string str = "SELECT * FROM tri WHERE Word1='a' AND Word2='b' AND Word3='c';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Fourgrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Fourgram, _wordList);

            const string str = "SELECT * FROM four WHERE Word1='a' AND Word2='b' AND Word3='c' AND Word4='d';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_NullListWithNames()
        {
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetSimilarNgramsFromTable(NgramType.Digram, null));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_WrongListSize()
        {
            var list = new List<string>
            {
                "a",
                "b"
            };

            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetSimilarNgramsFromTable(NgramType.Fourgram, list));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_GoodListSize()
        {
            var list = new List<string>
            {
                "a",
                "b",
                "c"
            };

            var provider = new SqlQueryProvider(_names);
            var ex = Record.Exception(() => provider.GetSimilarNgramsFromTable(NgramType.Fourgram, list));

            Assert.IsNotType<ArgumentException>(ex);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Unigrams_WrongNgramType()
        {
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetSimilarNgramsFromTable(NgramType.Unigram, _wordList));
            Assert.Equal("NgramType 'ngramType' cannot be an Unigram", ex.Message);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Digrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Digram, _wordList);

            const string str = "SELECT * FROM di WHERE Word1='a';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Trigrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Trigram, _wordList);

            const string str = "SELECT * FROM tri WHERE Word1='a' AND Word2='b';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Fourgrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Fourgram, _wordList);

            const string str = "SELECT * FROM four WHERE Word1='a' AND Word2='b' AND Word3='c';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetMultiNgramsFromTable_NullListWithWords()
        {
            var provider = new SqlQueryProvider(_names);
            var wordList = new List<string>
            {
                "a",
                "b",
                "c"
            };

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Fourgram, wordList, null));
            Assert.Equal("List<string> 'combinations' has wrong size", ex.Message);
            ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Fourgram, null, wordList));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetMultiNgramsFromTable_WrongListSize()
        {
            var list = new List<string>
            {
                "a",
                "b"
            };
            var wordList = new List<string>
            {
                "a",
                "b",
                "c"
            };
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Fourgram, wordList, new List<string>()));
            Assert.Equal("List<string> 'combinations' has wrong size", ex.Message);
            ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Fourgram, list, _wordList));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetMultiNgramsFromTable_GoodListSize()
        {
            var list = new List<string>
            {
                "a",
                "b",
                "c"
            };

            var wordList = new List<string>
            {
                "a",
                "b",
                "c"
            };

            var provider = new SqlQueryProvider(_names);
            var ex = Record.Exception(() => provider.GetMultiNgramsFromTable(NgramType.Fourgram, wordList, list));

            Assert.IsNotType<ArgumentException>(ex);
        }

        [Fact]
        public void GetMultiNgramsFromTable_Unigrams_WrongNgramType()
        {
            var provider = new SqlQueryProvider(_names);
            var wordList = new List<string>
            {
                "a",
                "b",
                "c"
            };

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Unigram, wordList, _wordList));
            Assert.Equal("NgramType 'ngramType' cannot be an Unigram", ex.Message);
        }

        [Fact]
        public void GetMultiNgramsFromTable_Fourgrams()
        {
            var list =new List<string>
            {
                "or1",
                "or2"
            };

            var wordList = new List<string>
            {
                "a",
                "b",
                "c"
            };

            var provider = new SqlQueryProvider(_names);
            var result = provider.GetMultiNgramsFromTable(NgramType.Fourgram, wordList, list);

            const string str = "SELECT * FROM four WHERE Word1='a' AND Word2='b' AND Word3='c' AND ( Word4='or1' OR Word4='or2' );";
            Assert.Equal(str, result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NgramAnalyzer.Common;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class SqlQueryProviderTests2
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
            @"a\",
            "b",
            @"'c",
            "d"
        };

        [Fact]
        public void SqlQueryProvider2_NullListWithNames()
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new SqlQueryProvider2(null));
            Assert.Equal("IList<string> 'dbTableNames' has wrong size", ex.Message);
        }

        [Fact]
        public void SqlQueryProvider2_WrongListSize()
        {
            var list = new List<string>
            {
                "uni",
                "di",
                "tri",
            };
            Exception ex = Assert.Throws<ArgumentException>(() => new SqlQueryProvider2(list));
            Assert.Equal("IList<string> 'dbTableNames' has wrong size", ex.Message);
        }

        #region GetTheSameNgramsFromTable
        [Fact]
        public void GetTheSameNgramsFromTable_NullListWithNames()
        {
            var provider = new SqlQueryProvider2(_names);

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

            var provider = new SqlQueryProvider2(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetTheSameNgramsFromTable(NgramType.Digram, list));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Digrams()
        {
            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Digram, _wordList);

            const string str = @"SELECT * FROM di[a] WHERE Word1='a\\' AND Word2='b';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Trigrams()
        {
            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Trigram, _wordList);

            const string str = @"SELECT * FROM tri[a] WHERE Word1='a\\' AND Word2='b' AND Word3='\'c';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Fourgrams()
        {
            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Fourgram, _wordList);

            const string str = @"SELECT * FROM four[a] WHERE Word1='a\\' AND Word2='b' AND Word3='\'c' AND Word4='d';";
            Assert.Equal(str, result);
        }
        #endregion

        #region GetSimilarNgramsFromTable
        [Fact]
        public void GetSimilarNgramsFromTable_NullListWithNames()
        {
            var provider = new SqlQueryProvider2(_names);

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

            var provider = new SqlQueryProvider2(_names);

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

            var provider = new SqlQueryProvider2(_names);
            var ex = Record.Exception(() => provider.GetSimilarNgramsFromTable(NgramType.Fourgram, list));

            Assert.IsNotType<ArgumentException>(ex);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Digrams()
        {
            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Digram, _wordList);

            const string str = @"SELECT * FROM di[a] WHERE Word1='a\\';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Trigrams()
        {
            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Trigram, _wordList);

            const string str = @"SELECT * FROM tri[a] WHERE Word1='a\\' AND Word2='b';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Fourgrams()
        {
            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Fourgram, _wordList);

            const string str = @"SELECT * FROM four[a] WHERE Word1='a\\' AND Word2='b' AND Word3='\'c';";
            Assert.Equal(str, result);
        }
        #endregion

        #region GetMultiNgramsFromTable
        [Fact]
        public void GetMultiNgramsFromTable_NullListWithWords()
        {
            var provider = new SqlQueryProvider2(_names);
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
            var provider = new SqlQueryProvider2(_names);

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

            var provider = new SqlQueryProvider2(_names);
            var ex = Record.Exception(() => provider.GetMultiNgramsFromTable(NgramType.Fourgram, wordList, list));

            Assert.IsNotType<ArgumentException>(ex);
        }

        [Fact]
        public void GetMultiNgramsFromTable_Fourgrams()
        {
            var list = new List<string>
            {
                @"\or1",
                "or2"
            };

            var wordList = new List<string>
            {
                @"a\",
                "b",
                @"'c"
            };

            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetMultiNgramsFromTable(NgramType.Fourgram, wordList, list);

            const string str = @"SELECT * FROM four[a] WHERE Word1='a\\' AND Word2='b' AND Word3='\'c' AND ( Word4='\\or1' OR Word4='or2' );";
            Assert.Equal(str, result);
        }
        #endregion

        #region CheckWordsInUnigramFromTable
        [Fact]
        public void CheckWordsInUnigramFromTable_NormalExample()
        {
            var wordList = new List<string>
            {
                @"a\",
                "b",
                @"baa'"
            };

            var provider = new SqlQueryProvider2(_names);
            var result = provider.CheckWordsInUnigramFromTable(wordList);

            const string str = @"SELECT * FROM `uni[a]` WHERE Word1='a\\' UNION ALL SELECT * FROM `uni[b]` WHERE Word1='b' OR Word1='baa\'';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void CheckWordsInUnigramFromTable_NullList()
        {
            var provider = new SqlQueryProvider2(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.CheckWordsInUnigramFromTable(null));
            Assert.Equal("List<string> 'wordList' can't be null", ex.Message);
        }

        [Fact]
        public void CheckWordsInUnigramFromTable_EmptyList()
        {
            var list = new List<string>();
            var provider = new SqlQueryProvider2(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.CheckWordsInUnigramFromTable(list));
            Assert.Equal("List<string> 'wordList' can't be null", ex.Message);
        }
        #endregion

        #region GetAllNecessaryNgramsFromTable
        [Fact]
        public void GetAllNecessaryNgramsFromTable_NormalExample()
        {
            var wordLists = new List<List<List<string>>>
            {
                new List<List<string>>
                {
                    new List<string>{@"a\","b"},
                    new List<string>{"c","d"},
                    new List<string>{"e",@"'f","g"}
                },
                new List<List<string>>
                {
                    new List<string>{"a","x"},
                    new List<string>{"y"},
                    new List<string>{"w","v","u"}
                }
            };

            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, wordLists);

            const string str = "SELECT * FROM tri[a] WHERE ( " +
                               @"( Word1='a\\' OR Word1='b' ) AND ( Word2='c' OR Word2='d' ) AND ( Word3='e' OR Word3='\'f' OR Word3='g' ) ) " +
                               "OR ( ( Word1='a' OR Word1='x' ) AND ( Word2='y' ) AND ( Word3='w' OR Word3='v' OR Word3='u' ) );";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetAllNecessaryNgramsFromTable_NormalExample2()
        {
            var wordLists = new List<List<List<string>>>
            {
                new List<List<string>>
                {
                    new List<string>{@"a\","b"},
                    new List<string>{"c","d"},
                    new List<string>{"e",@"'f","g"}
                },
                new List<List<string>>
                {
                    new List<string>{"z","x"},
                    new List<string>{"y"},
                    new List<string>{"w","v","u"}
                }
            };

            var provider = new SqlQueryProvider2(_names);
            var result = provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, wordLists);

            const string str = "SELECT * FROM tri[a] WHERE ( " +
                               @"( Word1='a\\' OR Word1='b' ) AND ( Word2='c' OR Word2='d' ) AND ( Word3='e' OR Word3='\'f' OR Word3='g' ) );" +
                               "SELECT * FROM tri[z] WHERE ( " +
                               @"( Word1='z' OR Word1='x' ) AND ( Word2='y' ) AND ( Word3='w' OR Word3='v' OR Word3='u' ) );";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetAllNecessaryNgramsFromTable_outsideList_Null()
        {
            var provider = new SqlQueryProvider2(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, null));
            Assert.Equal("List<string> 'wordLists' has wrong size", ex.Message);
        }

        [Fact]
        public void GetAllNecessaryNgramsFromTable_outsideList_Empty()
        {
            var wordLists = new List<List<List<string>>>();

            var provider = new SqlQueryProvider2(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, wordLists));
            Assert.Equal("List<string> 'wordLists' has wrong size", ex.Message);
        }

        [Fact]
        public void GetAllNecessaryNgramsFromTable_MiddleList_Empty()
        {
            var wordLists = new List<List<List<string>>>
            {
                new List<List<string>>(),
                new List<List<string>>
                {
                    new List<string>{"z","x"},
                    new List<string>{"y"},
                    new List<string>{"w","v","u"}
                }
            };

            var provider = new SqlQueryProvider2(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, wordLists));
            Assert.Equal("List<string> middle list has wrong size", ex.Message);
        }

        [Fact]
        public void GetAllNecessaryNgramsFromTable_InsideList_Empty()
        {
            var wordLists = new List<List<List<string>>>
            {
                new List<List<string>>
                {
                    new List<string>{"a","b"},
                    new List<string>(),
                    new List<string>{"e","f","g"}
                },
                new List<List<string>>
                {
                    new List<string>{"z","x"},
                    new List<string>{"y"},
                    new List<string>{"w","v","u"}
                }
            };

            var provider = new SqlQueryProvider2(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, wordLists));
            Assert.Equal("List<string> inside list has wrong size", ex.Message);
        }
        #endregion

        #region CreateDbString
        [Fact]
        public void CreateDbString()
        {
            var provider = new SqlQueryProvider2();
            var result = provider.CreateDbString("name");

            const string str = "CREATE DATABASE IF NOT EXISTS `name` CHARACTER SET utf8 COLLATE utf8_polish_ci;";
            Assert.Equal(str, result);
        }
        #endregion

        #region CreateNgramsTableString
        [Fact]
        public void CreateNgramsTableString_Digram()
        {
            var provider = new SqlQueryProvider2();
            var result = provider.CreateNgramsTableString("DbName", "TableName", 2);

            var names = new[]
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
                "v", "w", "x", "y", "z", "other"
            };

            var str = names.Aggregate("", (current, name) => current +
                        ($"CREATE TABLE IF NOT EXISTS `DbName`.`TableName[{name}]` " +
                        "( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                        "`Value` INT NOT NULL, `Word1` VARCHAR(30) NOT NULL, " +
                        "`Word2` VARCHAR(30) NOT NULL ) ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;"));

            Assert.Equal(str, result);
        }
        #endregion

        #region InsertNgramsString
        [Theory]
        [InlineData("ć", "c")]
        [InlineData("ę", "e")]
        [InlineData("ł", "l")]
        [InlineData("ń", "n")]
        [InlineData("ó", "o")]
        [InlineData("ś", "s")]
        [InlineData("ź", "z")]
        [InlineData("ż", "z")]
        [InlineData(@"\", "other")]
        public void InsertNgramsString(string a, string b)
        {
            var ngrams = new List<NGram>
            {
                new NGram{Value = 10, WordsList = new List<string>{"a","b"}},
                new NGram{Value = 15, WordsList = new List<string>{"a","c"}},
                new NGram{Value = 20, WordsList = new List<string>{a,"d"}}
            };
            var provider = new SqlQueryProvider2();
            var result = provider.InsertNgramsString("TableName", ngrams);

            string str = "INSERT INTO `TableName[a]` (`Value`, `Word1`, `Word2`) " +
                        "VALUES('10', 'a', 'b'),('15', 'a', 'c');" +
                        $"INSERT INTO `TableName[{b}]` (`Value`, `Word1`, `Word2`) " +
                        $"VALUES('20', '{a}', 'd');";
            Assert.Equal(str, result);
        }
        #endregion

        #region InsertOrUpdateNgramString
        [Fact]
        public void InsertOrUpdateNgramString()
        {
            var ngram = new NGram { Value = 10, WordsList = new List<string> { "ą", "b" } };

            var provider = new SqlQueryProvider2();
            var result = provider.InsertOrUpdateNgramString(ngram);

            const string str = "CALL `Add2gram[a]`('10', 'ą', 'b');";
            Assert.Equal(str, result);
        }
        #endregion

        #region CreateAddProcedureString
        [Fact]
        public void CreateAddProcedureString_Digram()
        {
            var provider = new SqlQueryProvider2();
            var result = provider.CreateAddProcedureString("BaseName", "TableName", 2);

            var names = new[]
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
                "v", "w", "x", "y", "z", "other"
            };

            var str = names.Aggregate("", (current, name) => current +
                    ($"DROP PROCEDURE IF EXISTS BaseName.`Add2gram[{name}]`; CREATE PROCEDURE BaseName.`Add2gram[{name}]`(in _value int, in _word1 varchar(30), in _word2 varchar(30)) " +
                    $"BEGIN SELECT @id:=ID, @val:=Value FROM BaseName.`TableName[{name}]` WHERE Word1 = _word1 AND Word2 = _word2; " +
                    $"IF @id IS NULL THEN INSERT INTO BaseName.`TableName[{name}]` (Value, Word1, Word2) VALUES ( _value, _word1, _word2); " +
                    $"ELSE UPDATE BaseName.`TableName[{name}]` SET Value = @val + _value WHERE ID = @id; END IF; END; "));

            Assert.Equal(str, result);
        }
        #endregion
    }
}

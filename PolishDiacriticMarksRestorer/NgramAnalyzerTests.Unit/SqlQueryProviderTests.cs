using System;
using System.Collections.Generic;
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
            @"\a",
            "b",
            @"'c",
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

        #region GetTheSameNgramsFromTable
        [Fact]
        public void GetTheSameNgramsFromTable_NullListWithNames()
        {
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetTheSameNgramsFromTable(NgramType.Bigram, null));
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

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetTheSameNgramsFromTable(NgramType.Bigram, list));
            Assert.Equal("List<string> 'wordList' has wrong size", ex.Message);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Digrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Bigram, _wordList);

            const string str = @"SELECT * FROM di WHERE Word1='\\a' AND Word2='b'";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Trigrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Trigram, _wordList);

            const string str = @"SELECT * FROM tri WHERE Word1='\\a' AND Word2='b' AND Word3='\'c'";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetTheSameNgramsFromTable_Fourgrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetTheSameNgramsFromTable(NgramType.Quadrigram, _wordList);

            const string str = @"SELECT * FROM four WHERE Word1='\\a' AND Word2='b' AND Word3='\'c' AND Word4='d'";
            Assert.Equal(str, result);
        }
        #endregion

        #region GetSimilarNgramsFromTable
        [Fact]
        public void GetSimilarNgramsFromTable_NullListWithNames()
        {
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetSimilarNgramsFromTable(NgramType.Bigram, null));
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

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetSimilarNgramsFromTable(NgramType.Quadrigram, list));
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
            var ex = Record.Exception(() => provider.GetSimilarNgramsFromTable(NgramType.Quadrigram, list));

            Assert.IsNotType<ArgumentException>(ex);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Digrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Bigram, _wordList);

            const string str = @"SELECT * FROM di WHERE Word1='\\a';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Trigrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Trigram, _wordList);

            const string str = @"SELECT * FROM tri WHERE Word1='\\a' AND Word2='b';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetSimilarNgramsFromTable_Fourgrams()
        {
            var provider = new SqlQueryProvider(_names);
            var result = provider.GetSimilarNgramsFromTable(NgramType.Quadrigram, _wordList);

            const string str = @"SELECT * FROM four WHERE Word1='\\a' AND Word2='b' AND Word3='\'c';";
            Assert.Equal(str, result);
        }
        #endregion

        #region GetMultiNgramsFromTable
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

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Quadrigram, wordList, null));
            Assert.Equal("List<string> 'combinations' has wrong size", ex.Message);
            ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Quadrigram, null, wordList));
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

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Quadrigram, wordList, new List<string>()));
            Assert.Equal("List<string> 'combinations' has wrong size", ex.Message);
            ex = Assert.Throws<ArgumentException>(() => provider.GetMultiNgramsFromTable(NgramType.Quadrigram, list, _wordList));
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
            var ex = Record.Exception(() => provider.GetMultiNgramsFromTable(NgramType.Quadrigram, wordList, list));

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
                @"\a",
                "b",
                @"'c"
            };

            var provider = new SqlQueryProvider(_names);
            var result = provider.GetMultiNgramsFromTable(NgramType.Quadrigram, wordList, list);

            const string str = @"SELECT * FROM four WHERE Word1='\\a' AND Word2='b' AND Word3='\'c' AND ( Word4='\\or1' OR Word4='or2' );";
            Assert.Equal(str, result);
        }
        #endregion

        #region CheckWordsInUnigramFromTable
        [Fact]
        public void CheckWordsInUnigramFromTable_NormalExample()
        {
            var wordList = new List<string>
            {
                @"\a",
                "b",
                @"'c"
            };

            var provider = new SqlQueryProvider(_names);
            var result = provider.CheckWordsInUnigramFromTable(wordList);

            const string str = @"SELECT * FROM uni WHERE Word1='\\a' OR Word1='b' OR Word1='\'c';";
            Assert.Equal(str, result);
        }

        [Fact]
        public void CheckWordsInUnigramFromTable_NullList()
        {
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.CheckWordsInUnigramFromTable(null));
            Assert.Equal("List<string> 'wordList' can't be null", ex.Message);
        }

        [Fact]
        public void CheckWordsInUnigramFromTable_EmptyList()
        {
            var list = new List<string>();
            var provider = new SqlQueryProvider(_names);

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
                    new List<string>{@"\a","b"},
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

            var provider = new SqlQueryProvider(_names);
            var result = provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, wordLists);

            const string str = "SELECT * FROM tri WHERE ( " +
                               @"( Word1='\\a' OR Word1='b' ) AND ( Word2='c' OR Word2='d' ) AND ( Word3='e' OR Word3='\'f' OR Word3='g' ) ) " +
                               "OR ( ( Word1='z' OR Word1='x' ) AND ( Word2='y' ) AND ( Word3='w' OR Word3='v' OR Word3='u' ) );";
            Assert.Equal(str, result);
        }

        [Fact]
        public void GetAllNecessaryNgramsFromTable_outsideList_Null()
        {
            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, null));
            Assert.Equal("List<string> 'wordLists' has wrong size", ex.Message);
        }

        [Fact]
        public void GetAllNecessaryNgramsFromTable_outsideList_Empty()
        {
            var wordLists = new List<List<List<string>>>();

            var provider = new SqlQueryProvider(_names);

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

            var provider = new SqlQueryProvider(_names);

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

            var provider = new SqlQueryProvider(_names);

            Exception ex = Assert.Throws<ArgumentException>(() => provider.GetAllNecessaryNgramsFromTable(NgramType.Trigram, wordLists));
            Assert.Equal("List<string> inside list has wrong size", ex.Message);
        }
        #endregion

        #region CreateDbString
        [Fact]
        public void CreateDbString()
        {
            var provider = new SqlQueryProvider();
            var result = provider.CreateDbString("name");

            const string str = "CREATE DATABASE IF NOT EXISTS `name` CHARACTER SET utf8 COLLATE utf8_polish_ci;";
            Assert.Equal(str, result);
        }
        #endregion

        #region CreateNgramsTableString
        [Fact]
        public void CreateNgramsTableString_Digram()
        {
            var provider = new SqlQueryProvider();
            var result = provider.CreateNgramsTableString("DbName", "TableName", 2);

            const string str = "CREATE TABLE IF NOT EXISTS `DbName`.`TableName` " +
                               "( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                               "`Value` INT NOT NULL, `Word1` VARCHAR(30) NOT NULL, " +
                               "`Word2` VARCHAR(30) NOT NULL ) ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;";
            Assert.Equal(str, result);
        }
        #endregion

        #region InsertNgramsString
        [Fact]
        public void InsertNgramsString()
        {
            var ngrams = new List<NGram>
            {
                new NGram(10, new List<string>{"a","b"}),
                new NGram(20, new List<string>{"c","d"})
            };
            var provider = new SqlQueryProvider();
            var result = provider.InsertNgramsString("TableName", ngrams);

            const string str = "INSERT INTO `TableName` (`Value`, `Word1`, `Word2`) " +
                "VALUES('10', 'a', 'b'),('20', 'c', 'd');";
            Assert.Equal(str, result);
        }
        #endregion

        #region InsertOrUpdateNgramString
        [Fact]
        public void InsertOrUpdateNgramString()
        {
            var ngram = new NGram(10, new List<string> { "a", "b" });

            var provider = new SqlQueryProvider();
            var result = provider.InsertOrUpdateNgramString(ngram);

            const string str = "CALL Add2gram('10', 'a', 'b');";
            Assert.Equal(str, result);
        }
        #endregion

        #region CreateAddProcedureString
        [Fact]
        public void CreateAddProcedureString_Digram()
        {
            var provider = new SqlQueryProvider();
            var result = provider.CreateAddProcedureString("BaseName", "TableName", 2);

            const string str = "DROP PROCEDURE IF EXISTS BaseName.Add2gram; CREATE PROCEDURE BaseName.Add2gram(in _value int, in _word1 varchar(30), in _word2 varchar(30)) " +
                               "BEGIN SELECT @id:=ID, @val:=Value FROM BaseName.TableName WHERE Word1 = _word1 AND Word2 = _word2; " +
                               "IF @id IS NULL THEN INSERT INTO BaseName.TableName (Value, Word1, Word2) VALUES ( _value, _word1, _word2); " +
                               "ELSE UPDATE BaseName.TableName SET Value = @val + _value WHERE ID = @id; END IF; END";
            Assert.Equal(str, result);
        }
        #endregion
    }
}

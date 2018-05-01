using System;
using System.Collections.Generic;
using System.Linq;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// SqlQueryProvider2 Class provides MySql Query.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IQueryProvider" />
    public class SqlQueryProvider2 : IQueryProvider
    {
        #region FIELDS
        private readonly IList<string> _dbTableDbTableName;
        public static readonly string[] Names = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other" };
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryProvider"/> class.
        /// </summary>
        /// <param name="dbTableNames">The database table names.</param>
        /// <exception cref="ArgumentException">IList (string) 'dbTableNames' has wrong size</exception>
        public SqlQueryProvider2(IList<string> dbTableNames)
        {
            if (dbTableNames == null || dbTableNames.Count != 4)
                throw new ArgumentException("IList<string> 'dbTableNames' has wrong size");
            _dbTableDbTableName = dbTableNames;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryProvider2"/> class.
        /// </summary>
        public SqlQueryProvider2() { }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// Generate query to get data from Ngrams table.
        /// </summary>
        /// <param name="ngramType">Type of NGrams.</param>
        /// <param name="wordList">List of words - must have a size suitable for the type ngrams.</param>
        /// <returns>
        /// Query string.
        /// </returns>
        /// <exception cref="ArgumentException">List(string) 'wordList' has wrong size</exception>
        /// <inheritdoc />
        public string GetTheSameNgramsFromTable(NgramType ngramType, List<string> wordList)
        {
            var number = (int)ngramType;

            if (wordList == null || wordList.Count < number)
                throw new ArgumentException("List<string> 'wordList' has wrong size");

            var index = GetIndexOfNames(wordList[0]);

            var query = $"SELECT * FROM `{_dbTableDbTableName[number - 1]}[{Names[index]}]` WHERE";

            for (var i = 0; i < number; ++i)
            {
                if (i != 0) query += " AND";
                query += " Word" + (i + 1) + "='" + wordList[i].ChangeSpecialCharacters() + "'";
            }

            //query += ";";

            return query;
        }

        /// <summary>
        /// Generate query which gets the ngrams changing the last word from table.
        /// </summary>
        /// <param name="ngramType">Type of the ngram.</param>
        /// <param name="wordList">The word list.</param>
        /// <param name="combinations">Possible last words.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// List(string) 'wordList' has wrong size
        /// or
        /// NgramType 'ngramType' cannot be an Unigram
        /// </exception>
        public string GetMultiNgramsFromTable(NgramType ngramType, List<string> wordList, List<string> combinations)
        {
            var number = (int)ngramType;

            if (wordList == null || wordList.Count != number - 1)
                throw new ArgumentException("List<string> 'wordList' has wrong size");
            if (combinations == null || combinations.Count < 1)
                throw new ArgumentException("List<string> 'combinations' has wrong size");

            var index = GetIndexOfNames(wordList[0]);

            var query = $"SELECT * FROM `{_dbTableDbTableName[number - 1]}[{Names[index]}]` WHERE ";

            for (var i = 0; i < number - 1; ++i)
            {
                if (i != 0) query += "AND ";
                query += "Word" + (i + 1) + "='" + wordList[i].ChangeSpecialCharacters() + "' ";
            }
            query += "AND ( ";
            for (var i = 0; i < combinations.Count; ++i)
            {
                if (i != 0) query += "OR ";
                query += "Word" + number + "='" + combinations[i].ChangeSpecialCharacters() + "' ";
            }
            query += ");";

            return query;
        }

        /// <inheritdoc />
        /// <summary>
        /// Generate query to get the similar ngrams from Ngrams table.
        /// </summary>
        /// <param name="ngramType">Type of the ngram.</param>
        /// <param name="wordList">List of words - must have a one size smaller than the type ngrams.</param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentException">
        /// List(string) 'wordList' has wrong size
        /// or
        /// NgramType 'ngramType' cannot be an Unigram
        /// </exception>
        public string GetSimilarNgramsFromTable(NgramType ngramType, List<string> wordList)
        {
            var number = (int)ngramType;
            var numberComparedWords = number - 1;

            if (wordList == null || wordList.Count < numberComparedWords)
                throw new ArgumentException("List<string> 'wordList' has wrong size");

            var query = QueryCreator(number, numberComparedWords, wordList);

            return query;
        }

        public string CheckWordsInUnigramFromTable(List<string> wordList)
        {
            if (wordList == null || wordList.Count == 0)
                throw new ArgumentException("List<string> 'wordList' can't be null");

            var comandsText = new string[Names.Length];
            for (var index = 0; index < comandsText.Length; index++)
            {
                comandsText[index] = "";
            }

            foreach (var item in wordList)
            {
                var index = GetIndexOfNames(item);
                if (comandsText[index] == "")
                {
                    comandsText[index] = "SELECT * FROM `" + _dbTableDbTableName[0] + "[" + Names[index] + "]` WHERE";
                    comandsText[index] += " Word1='" + item.ChangeSpecialCharacters() + "'";
                }
                else
                {
                    comandsText[index] += " OR";
                    comandsText[index] += " Word1='" + item.ChangeSpecialCharacters() + "'";
                }
            }

            var query = comandsText.Where(item => item != "").Aggregate("", (current, item) => current + (item + " UNION ALL "));
            query = query.Substring(0, query.Length - 11);
            query += ";";
            return query;
        }

        public string GetAllNecessaryNgramsFromTable(NgramType ngramType, List<List<List<string>>> wordLists)
        {
            var number = (int)ngramType;

            if (wordLists == null || wordLists.Count < 1)
                throw new ArgumentException("List<string> 'wordLists' has wrong size");
            foreach (var item in wordLists)
            {
                if (item.Count != number)
                    throw new ArgumentException("List<string> middle list has wrong size");
                foreach (var item2 in item)
                {
                    if (item2.Count < 1)
                        throw new ArgumentException("List<string> inside list has wrong size");
                }
            }

            var commandsText = new string[Names.Length];
            for (var index = 0; index < commandsText.Length; index++)
            {
                commandsText[index] = "";
            }

            foreach (var item1 in wordLists)
            {
                var index = GetIndexOfNames(item1[0][0]);
                if (commandsText[index] == "")
                {
                    commandsText[index] = $"SELECT * FROM `{_dbTableDbTableName[number - 1]}[{Names[index]}]` WHERE ";
                }
                else
                {
                    commandsText[index] += " OR ";
                }

                var j = 1;

                commandsText[index] += "( ";
                foreach (var item2 in item1)
                {
                    if (j != 1) commandsText[index] += " AND ";
                    commandsText[index] += "( ";

                    for (var i = 0; i < item2.Count; ++i)
                    {
                        if (i != 0) commandsText[index] += "OR ";
                        commandsText[index] += "Word" + j + "='" + item2[i].ChangeSpecialCharacters() + "' ";
                    }

                    commandsText[index] += ")";
                    ++j;
                }
                commandsText[index] += " )";
            }

            var result = "";
            for (var index = 0; index < commandsText.Length; index++)
            {
                if (commandsText[index] != "") commandsText[index] += " UNION ALL ";
                result += commandsText[index];
            }

            return result.Substring(0, result.Length - 11) + ";";
        }

        public string CreateDbString(string name)
        {
            return $"CREATE DATABASE IF NOT EXISTS `{name}` CHARACTER SET utf8 COLLATE utf8_polish_ci;";
        }

        public string CreateNgramsTableString(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText = "";

            foreach (var item in Names)
            {
                commandText += $"CREATE TABLE IF NOT EXISTS `{dataBaseName}`.`{tableName}[{item}]` " +
                               "( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY, " + "`Value` INT NOT NULL";

                for (var i = 0; i < numberOfWords; ++i)
                {
                    commandText += $", `Word{i + 1}` VARCHAR(30) NOT NULL";
                }

                commandText += " ) ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;";
            }

            return commandText;
        }

        public string InsertNgramsString(string tableName, List<NGram> ngrams)
        {
            var comandsText = new string[Names.Length];
            for (var index = 0; index < comandsText.Length; index++)
            {
                comandsText[index] = "";
            }

            foreach (var item in ngrams)
            {
                var index = GetIndexOfNames(item.WordsList[0]);
                if (comandsText[index] == "")
                {
                    comandsText[index] = $"INSERT INTO `{tableName}[{Names[index]}]` (`Value`";

                    for (var i = 0; i < ngrams[0].WordsList.Count; ++i)
                    {
                        comandsText[index] += $", `Word{i + 1}`";
                    }

                    comandsText[index] += ") VALUES";
                }
                comandsText[index] += $"('{item.Value}'";
                foreach (var elem in item.WordsList)
                {
                    comandsText[index] += $", '{elem}'";
                }
                comandsText[index] += "),";
            }

            return comandsText.Where(item => item != "")
                .Select(text => new System.Text.StringBuilder(text) { [text.Length - 1] = ';' })
                .Aggregate("", (current, strBuilder) => current + strBuilder.ToString());
        }

        public string IndexingWords(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText = "";

            foreach (var item in Names)
            {
                for (var i = 0; i < numberOfWords; ++i)
                {
                    commandText += $"ALTER TABLE `{dataBaseName}`.`{tableName}[{item}]` ADD INDEX (`Word{i + 1}`);";
                }
            }

            return commandText;
        }

        public string InsertOrUpdateNgramString(NGram ngram)
        {
            var index = GetIndexOfNames(ngram.WordsList[0]);
            var count = ngram.WordsList.Count;

            var commandText =
                string.Format("CALL `Add{0}gram[{2}]`('{1}'", count, ngram.Value, Names[index]);

            foreach (var item in ngram.WordsList)
            {
                commandText += $", '{item}'";
            }

            commandText += ");";

            return commandText;

        }

        public string CreateAddProcedureString(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandsText = "";

            foreach (var item in Names)
            {
                var commandText =
                    string.Format("DROP PROCEDURE IF EXISTS {0}.`Add{1}gram[{2}]`; CREATE PROCEDURE {0}.`Add{1}gram[{2}]`(in _value int",
                        dataBaseName, numberOfWords, item);

                for (var i = 0; i < numberOfWords; ++i)
                {
                    commandText += $", in _word{i + 1} varchar(30)";
                }

                commandText += ") BEGIN SELECT @id:=ID, @val:=Value FROM " + dataBaseName + ".`" + tableName + "[" + item + "]` WHERE ";

                for (var i = 0; i < numberOfWords; ++i)
                {
                    if (i != 0) commandText += " AND ";
                    commandText += string.Format("Word{0} = _word{0}", i + 1);
                }

                commandText += "; IF @id IS NULL THEN INSERT INTO " + dataBaseName + ".`" + tableName + "[" + item + "]` (Value";

                for (var i = 0; i < numberOfWords; ++i)
                {
                    commandText += $", Word{i + 1}";
                }

                commandText += ") VALUES ( _value";

                for (var i = 0; i < numberOfWords; ++i)
                {
                    commandText += $", _word{i + 1}";
                }

                commandText += "); ELSE UPDATE " + dataBaseName + ".`" + tableName + "[" + item + "]` SET Value = @val + _value WHERE ID = @id; END IF; END; ";
                commandsText += commandText;
            }

            return commandsText;
        }
        #endregion

        #region PRIVATE
        private string QueryCreator(int ngramSize, int numberComparedWords, IReadOnlyList<string> wordList)
        {
            var index = GetIndexOfNames(wordList[0]);

            var query = $"SELECT * FROM {_dbTableDbTableName[ngramSize - 1]}[{Names[index]}] WHERE";

            for (var i = 0; i < numberComparedWords; ++i)
            {
                if (i != 0) query += " AND";
                query += " Word" + (i + 1) + "='" + wordList[i].ChangeSpecialCharacters() + "'";
            }

            query += ";";

            return query;
        }

        private int GetIndexOfNames(string str)
        {
            string tmp;
            switch (str[0])
            {
                case 'ą':
                    tmp = "a";
                    break;
                case 'ć':
                    tmp = "c";
                    break;
                case 'ę':
                    tmp = "e";
                    break;
                case 'ł':
                    tmp = "l";
                    break;
                case 'ń':
                    tmp = "n";
                    break;
                case 'ó':
                    tmp = "o";
                    break;
                case 'ś':
                    tmp = "s";
                    break;
                case 'ż':
                case 'ź':
                    tmp = "z";
                    break;
                case char n when (n >= 'a' && n <= 'z'):
                    tmp = n.ToString();
                    break;
                default:
                    tmp = "other";
                    break;
            }

            var index = -1;
            for (var i = 0; i < Names.Length; i++)
            {
                if (!Names[i].Equals(tmp)) continue;
                index = i;
                break;
            }

            return index;
        }
        #endregion
    }
}

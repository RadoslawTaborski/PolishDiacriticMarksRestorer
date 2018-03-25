using System;
using System.Collections.Generic;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// SqlQueryProvider Class provides MySql Query.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IQueryProvider" />
    public class SqlQueryProvider : IQueryProvider
    {
        #region FIELDS
        private readonly IList<string> _dbTableDbTableName;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryProvider"/> class.
        /// </summary>
        /// <param name="dbTableNames">The database table names.</param>
        /// <exception cref="ArgumentException">IList (string) 'dbTableNames' has wrong size</exception>
        public SqlQueryProvider(IList<string> dbTableNames)
        {
            if (dbTableNames == null || dbTableNames.Count != 4)
                throw new ArgumentException("IList<string> 'dbTableNames' has wrong size");
            _dbTableDbTableName = dbTableNames;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryProvider"/> class.
        /// </summary>
        public SqlQueryProvider() { }
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

            var query = "SELECT * FROM " + _dbTableDbTableName[number - 1] + " WHERE";

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

            var query = "SELECT * FROM " + _dbTableDbTableName[number - 1] + " WHERE ";

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

            var query = "SELECT * FROM " + _dbTableDbTableName[0] + " WHERE";

            for (var i = 0; i < wordList.Count; ++i)
            {
                if (i != 0) query += " OR";
                query += " Word1='" + wordList[i].ChangeSpecialCharacters() + "'";
            }

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


            var query = "SELECT * FROM " + _dbTableDbTableName[number - 1] + " WHERE ";

            var z = 1;
            foreach (var item1 in wordLists)
            {
                var j = 1;

                if (z != 1) query += " OR ";

                query += "( ";
                foreach (var item2 in item1)
                {
                    if (j != 1) query += " AND ";
                    query += "( ";

                    for (var i = 0; i < item2.Count; ++i)
                    {
                        if (i != 0) query += "OR ";
                        query += "Word" + j + "='" + item2[i].ChangeSpecialCharacters() + "' ";
                    }

                    query += ")";
                    ++j;
                }
                query += " )";
                ++z;
            }

            query += ";";

            return query;
        }

        public string CreateDbString(string name)
        {
            return $"CREATE DATABASE IF NOT EXISTS `{name}` CHARACTER SET utf8 COLLATE utf8_polish_ci;";
        }

        public string CreateNgramsTableString(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText = $"CREATE TABLE IF NOT EXISTS `{dataBaseName}`.`{tableName}` " +
                              "( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY, " + "`Value` INT NOT NULL";

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += $", `Word{i + 1}` VARCHAR(30) NOT NULL";
            }

            commandText += " ) ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;";

            return commandText;
        }

        public string InsertNgramsString(string tableName, List<NGram> ngrams)
        {
            var commandText = $"INSERT INTO `{tableName}` (`Value`";

            for (var i = 0; i < ngrams[0].WordsList.Count; ++i)
            {
                commandText += $", `Word{i + 1}`";
            }

            commandText += ") VALUES";

            foreach (var item in ngrams)
            {
                commandText += $"('{item.Value}'";
                foreach (var elem in item.WordsList)
                {
                    commandText += $", '{elem}'";
                }
                commandText += "),";
            }

            var strBuilder =
                new System.Text.StringBuilder(commandText) { [commandText.Length - 1] = ';' };
            commandText = strBuilder.ToString();

            return commandText;
        }

        public string IndexingWords(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText = "";

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += $"ALTER TABLE `{dataBaseName}`.`{tableName}` ADD INDEX (`Word{i + 1}`);";
            }

            return commandText;
        }

        public string InsertOrUpdateNgramString(NGram ngram)
        {
            var count = ngram.WordsList.Count;

            var commandText =
                $"CALL Add{count}gram('{ngram.Value}'";

            foreach (var item in ngram.WordsList)
            {
                commandText += $", '{item}'";
            }

            commandText += ");";

            return commandText;
        }

        public string CreateAddProcedureString(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText =
                string.Format("DROP PROCEDURE IF EXISTS {0}.Add{1}gram; CREATE PROCEDURE {0}.Add{1}gram(in _value int",
                    dataBaseName, numberOfWords);

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += $", in _word{i + 1} varchar(30)";
            }

            commandText += ") BEGIN SELECT @id:=ID, @val:=Value FROM " + dataBaseName + "." + tableName + " WHERE ";

            for (var i = 0; i < numberOfWords; ++i)
            {
                if (i != 0) commandText += " AND ";
                commandText += string.Format("Word{0} = _word{0}", i + 1);
            }

            commandText += "; IF @id IS NULL THEN INSERT INTO " + dataBaseName + "." + tableName + " (Value";

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += $", Word{i + 1}";
            }

            commandText += ") VALUES ( _value";

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += $", _word{i + 1}";
            }

            commandText += "); ELSE UPDATE " + dataBaseName + "." + tableName + " SET Value = @val + _value WHERE ID = @id; END IF; END";

            return commandText;
        }
        #endregion

        #region PRIVATE
        private string QueryCreator(int ngramSize, int numberComparedWords, IReadOnlyList<string> wordList)
        {
            var query = "SELECT * FROM " + _dbTableDbTableName[ngramSize - 1] + " WHERE";

            for (var i = 0; i < numberComparedWords; ++i)
            {
                if (i != 0) query += " AND";
                query += " Word" + (i + 1) + "='" + wordList[i].ChangeSpecialCharacters() + "'";
            }

            query += ";";

            return query;
        }
        #endregion
    }
}

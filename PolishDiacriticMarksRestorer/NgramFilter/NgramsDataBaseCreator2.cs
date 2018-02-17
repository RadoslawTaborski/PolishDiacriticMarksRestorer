using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    internal class NgramsDataBaseCreator2 : IDataBaseCreator
    {
        #region FIELDS
        private readonly IDataAccess _db;
        public static readonly string[] Names = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "other"};

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="NgramsDataBaseCreator2"/> class.
        /// </summary>
        /// <param name="db">The data access class.</param>
        public NgramsDataBaseCreator2(IDataAccess db)
        {
            _db = db;
        }
        #endregion

        #region PUBLIC
        public void CreateDataBase(string name)
        {
            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(CreateDbString(name));
            _db.Disconnect();
        }

        public void CreateTables(string dataBaseName, string tableName, int numberOfWords)
        {
            if (numberOfWords < 1) return;

            var commandText = CreateNgramsTableString(dataBaseName, tableName, numberOfWords);
            //var commandText1 = CreateAddProcedureString(dataBaseName, tableName, numberOfWords);

            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(commandText);
            //_db.ExecuteNonQueryServer(commandText1);
            _db.Disconnect();
        }

        public void AddNgramsToTable(string tableName, List<NGram> ngrams)
        {
            if (ngrams == null || ngrams.Count <= 0) return;

            var commandText = InsertNgramsString(tableName, ngrams);

            _db.ConnectToDb();
            _db.ExecuteNonQueryDb(commandText);
            _db.Disconnect();
        }

        public void AddOrUpdateNgramsToTable(string tableName, List<NGram> ngrams)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PRIVATE
        private string CreateDbString(string name)
        {
            return string.Format("CREATE DATABASE IF NOT EXISTS `{0}` CHARACTER SET utf8 COLLATE utf8_polish_ci;",
                name);
        }

        private string CreateNgramsTableString(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText = "";

            foreach (var item in Names)
            {
                commandText += string.Format(
                    "CREATE TABLE IF NOT EXISTS `{0}`.`{1}[{2}]` " +
                    "( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                    "`Value` INT NOT NULL", dataBaseName, tableName,item);

                for (var i = 0; i < numberOfWords; ++i)
                {
                    commandText += string.Format(", `Word{0}` VARCHAR(30) NOT NULL", i + 1);
                }

                commandText += " ) ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;";
            }

            return commandText;
        }

        private string InsertNgramsString(string tableName, List<NGram> ngrams)
        {
            var comandsText = new string[Names.Length];
            for (var index = 0; index < comandsText.Length; index++)
            {
                comandsText[index] = "";
            }

            foreach (var item in ngrams)
            {
                var index = GetIndexOfNames(item.WordsList[0]);
                if (index == -1) continue;
                if (comandsText[index] == "")
                {
                    comandsText[index] = string.Format(
                        "INSERT INTO `{0}[{1}]` (`Value`", tableName,Names[index]);

                    for (var i = 0; i < ngrams[0].WordsList.Count; ++i)
                    {
                        comandsText[index] += string.Format(", `Word{0}`", i + 1);
                    }

                    comandsText[index] += ") VALUES";
                }
                comandsText[index] += string.Format("('{0}'", item.Value);
                foreach (var elem in item.WordsList)
                {
                    comandsText[index] += string.Format(", '{0}'", elem);
                }
                comandsText[index] += "),";
            }

            return comandsText.Where(item => item != "")
                .Select(text => new System.Text.StringBuilder(text) {[text.Length - 1] = ';'})
                .Aggregate("", (current, strBuilder) => current + strBuilder.ToString());
        }

        private int GetIndexOfNames(string str)
        {
            string tmp;
            switch (str[0])
            {
                case 'ą':
                    tmp= "a";
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

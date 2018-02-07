using System;
using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    internal class NgramsDataBaseCreator : IDataBaseCreator
    {
        private readonly IDataAccess _db;

        public NgramsDataBaseCreator(IDataAccess db)
        {
            _db = db;
        }

        public void CreateDataBase(string name)
        {
            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(CreateDbString(name));
            _db.Disconnect();
        }

        public void CreateTable(string dataBaseName, string tableName, int numberOfWords)
        {
            if (numberOfWords < 1) return;

            var commandText = CreateNgramsTableString(dataBaseName, tableName, numberOfWords);

            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(commandText);
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

        private string CreateDbString(string name)
        {
            return string.Format("CREATE DATABASE IF NOT EXISTS `{0}` CHARACTER SET utf8 COLLATE utf8_polish_ci;",
                name);
        }

        private string CreateNgramsTableString(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText = string.Format(
                "CREATE TABLE IF NOT EXISTS `{0}`.`{1}` "+
                "( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY, "+
                "`Value` INT NOT NULL", dataBaseName, tableName);

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += string.Format(", `Word{0}` VARCHAR(30) NOT NULL", i + 1);
            }

            commandText += " ) ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;";

            return commandText;
        }

        private string InsertNgramsString(string tableName, List<NGram> ngrams)
        {
            var commandText = string.Format(
                "INSERT INTO `{0}` (`Value`", tableName);

            for (var i = 0; i < ngrams[0].WordsList.Count; ++i)
            {
                commandText += string.Format(", `Word{0}`", i + 1);
            }

            commandText += ") VALUES";

            foreach (var item in ngrams)
            {
                commandText += string.Format("('{0}'", item.Value);
                foreach (var elem in item.WordsList)
                {
                    commandText += string.Format(", '{0}'", elem);
                }
                commandText += "),";
            }

            var strBuilder =
                new System.Text.StringBuilder(commandText) { [commandText.Length - 1] = ';' };
            commandText = strBuilder.ToString();

            return commandText;
        }
    }
}

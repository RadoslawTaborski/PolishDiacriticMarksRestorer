using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    internal class DataBaseCreator : IDataBaseCreator
    {
        private readonly IDataAccess _db;

        public DataBaseCreator(IDataAccess db)
        {
            _db = db;
        }

        public void CreateDataBase(string name)
        {
            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(string.Format(@"CREATE DATABASE IF NOT EXISTS `{0}` CHARACTER SET utf8 COLLATE utf8_polish_ci;", name));
            _db.Disconnect();
        }

        public void CreateTable(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText = string.Format(
                @"CREATE TABLE IF NOT EXISTS `{0}`.`{1}` 
                ( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY , 
                `Value` INT NOT NULL ", dataBaseName, tableName);

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += string.Format(@", `Word{0}` VARCHAR(30) NOT NULL ", i + 1);
            }

            commandText += @") ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;";

            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(commandText);
            _db.Disconnect();
        }

        public void AddNgramsToTable(string tableName, List<NGram> ngrams)
        {
            if (ngrams == null || ngrams.Count <= 0) return;

            var commandText = string.Format(
                @"INSERT INTO `{0}` (`Value`", tableName);

            for (var i = 0; i < ngrams[0].WordsList.Count; ++i)
            {
                commandText += string.Format(@", `Word{0}`", i + 1);
            }

            commandText += @") VALUES";

            foreach (var item in ngrams)
            {
                commandText += string.Format(@"('{0}'", item.Value);
                foreach (var elem in item.WordsList)
                {
                    commandText += string.Format(@", '{0}'", elem);
                }
                commandText += "),";
            }

            var strBuilder =
                new System.Text.StringBuilder(commandText) {[commandText.Length-1] = ';'};
            commandText = strBuilder.ToString();

            _db.ConnectToDb();
            _db.ExecuteNonQueryDb(commandText);
            _db.Disconnect();
        }

        public void OpenDb()
        {

        }

        public void CloseDb()
        {

        }
    }
}

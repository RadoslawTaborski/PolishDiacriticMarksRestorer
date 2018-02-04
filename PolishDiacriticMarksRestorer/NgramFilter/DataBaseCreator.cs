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

        public void AddNgramToTable(string tableName, NGram ngram)
        {
            var commandText = string.Format(
                @"INSERT INTO `{0}` (`Value`", tableName);

            for (var i = 0; i < ngram.WordsList.Count; ++i)
            {
                commandText += string.Format(@", `Word{0}`", i + 1);
            }

            commandText += string.Format(@") VALUES ('{0}'", ngram.Value);

            foreach (var item in ngram.WordsList)
            {
                commandText += string.Format(@", '{0}'", item);
            }

            commandText += ")";
      
            _db.ExecuteNonQueryDb(commandText);
        }

        public void OpenDb()
        {
            _db.ConnectToDb();
        }

        public void CloseDb()
        {
            _db.Disconnect();
        }
    }
}

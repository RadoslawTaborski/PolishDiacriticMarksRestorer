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
            var commandText1 = CreateAddProcedureString(dataBaseName, tableName, numberOfWords);

            _db.ConnectToServer();
            _db.ExecuteNonQueryServer(commandText);
            _db.ExecuteNonQueryServer(commandText1);
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
            if (ngrams == null || ngrams.Count <= 0) return;

            _db.ConnectToDb();
            foreach (var item in ngrams)
            {
                var commandText = InsertOrUpdateNgramsString(item);
                _db.ExecuteNonQueryDb(commandText);
            }
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

        private string InsertOrUpdateNgramsString(NGram ngram)
        {
            var count = ngram.WordsList.Count;

            var commandText =
                string.Format("CALL Add{0}gram('{1}'",count,ngram.Value);

            foreach (var item in ngram.WordsList)
            {
                commandText += string.Format(", '{0}'", item);
            }

            commandText += ");";

            return commandText;
        }

        private string CreateAddProcedureString(string dataBaseName, string tableName, int numberOfWords)
        {
            var commandText =
                string.Format("DROP PROCEDURE IF EXISTS {0}.Add{1}gram; CREATE PROCEDURE {0}.Add{1}gram(in _value int",
                    dataBaseName, numberOfWords);

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += string.Format(", in _word{0} varchar(30)", i + 1);
            }

            commandText += ") BEGIN SELECT @id:=ID, @val:=Value FROM " + dataBaseName + "." + tableName +" WHERE ";

            for (var i = 0; i < numberOfWords; ++i)
            {
                if (i != 0) commandText += " AND ";
                commandText += string.Format("Word{0} = _word{0}", i + 1);
            }

            commandText += "; IF @id IS NULL THEN INSERT INTO " + dataBaseName + "." + tableName + " (Value";

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += string.Format(", Word{0}", i + 1);
            }

            commandText += ") VALUES ( _value";

            for (var i = 0; i < numberOfWords; ++i)
            {
                commandText += string.Format(", _word{0}", i + 1);
            }

            commandText += "); ELSE UPDATE " + dataBaseName + "." + tableName + " SET Value = @val + _value WHERE ID = @id; END IF; END";

            return commandText;
        }
    }
}

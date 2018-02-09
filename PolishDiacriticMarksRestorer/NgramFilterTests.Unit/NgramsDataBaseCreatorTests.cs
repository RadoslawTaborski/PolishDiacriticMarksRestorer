using System.Collections.Generic;
using Moq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using NgramFilter;
using Xunit;

namespace NgramFilterTests.Unit
{
    public class NgramsDataBaseCreatorTests
    {
        [Theory]
        [InlineData("name")]
        public void CreateDataBase_Verify(string name)
        {
            var commandText = "CREATE DATABASE IF NOT EXISTS `" +
                              name + "` CHARACTER SET utf8 COLLATE utf8_polish_ci;";

            var dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(m => m.ExecuteNonQueryServer(commandText)).Verifiable();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.CreateDataBase(name);

            dataAccessMock.Verify();
        }

        [Theory]
        [InlineData("dbName", "tableName")]
        public void CreateTable1Words_Verify(string dbName, string tableName)
        {
            var commandText = "CREATE TABLE IF NOT EXISTS `" + dbName + "`.`" +
                              tableName + "` ( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                              "`Value` INT NOT NULL, `Word1` VARCHAR(30) NOT NULL ) " +
                              "ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;";

            var dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(m => m.ExecuteNonQueryServer(commandText)).Verifiable();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.CreateTable(dbName, tableName, 1);

            dataAccessMock.Verify();
        }

        [Theory]
        [InlineData("dbName", "tableName")]
        public void CreateTable_NotCreate_Verify(string dbName, string tableName)
        {
            var dataAccessMock = new Mock<IDataAccess>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.CreateTable(dbName, tableName, -1);

            dataAccessMock.Verify(m => m.ExecuteNonQueryServer(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("dbName", "tableName")]
        public void CreateTable2Words_Verify(string dbName, string tableName)
        {
            var commandText = "CREATE TABLE IF NOT EXISTS `" + dbName + "`.`" +
                              tableName + "` ( `ID` INT NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                              "`Value` INT NOT NULL, `Word1` VARCHAR(30) NOT NULL, " +
                              "`Word2` VARCHAR(30) NOT NULL ) " +
                              "ENGINE = InnoDB CHARACTER SET utf8 COLLATE utf8_polish_ci;";

            var dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(m => m.ExecuteNonQueryServer(commandText)).Verifiable();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.CreateTable(dbName, tableName, 2);

            dataAccessMock.Verify();
        }

        [Theory]
        [InlineData("tableName")]
        public void AddNgramsToTable_Digrams_Verify(string tableName)
        {
            var ngrams = new List<NGram>
            {
                new NGram {Value = 10, WordsList = new List<string> {"small", "cat"}},
                new NGram {Value = 10, WordsList = new List<string> {"big", "cat"}}
            };

            var commandText = "INSERT INTO `" + tableName +
                              "` (`Value`, `Word1`, `Word2`) VALUES('" + ngrams[0].Value +
                              "', '" + ngrams[0].WordsList[0] + "', '" + ngrams[0].WordsList[1] +
                              "'),('" + ngrams[1].Value + "', '" + ngrams[1].WordsList[0] +
                              "', '" + ngrams[1].WordsList[1] + "');";

            var dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(m => m.ExecuteNonQueryDb(commandText)).Verifiable();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.AddNgramsToTable(tableName, ngrams);

            dataAccessMock.Verify();
        }

        [Theory]
        [InlineData("tableName")]
        public void AddOrUpdateNgramsToTable_Digrams_Verify(string tableName)
        {
            var ngrams = new List<NGram>
            {
                new NGram {Value = 10, WordsList = new List<string> {"small", "cat"}},
                new NGram {Value = 10, WordsList = new List<string> {"big", "cat"}}
            };

            var commandText = "CALL Add2gram('" + ngrams[0].Value +
                              "', '" + ngrams[0].WordsList[0] + "', '" + ngrams[0].WordsList[1] +
                              "');";

            var commandText1 = "CALL Add2gram('" + ngrams[1].Value +
                              "', '" + ngrams[1].WordsList[0] + "', '" + ngrams[1].WordsList[1] +
                              "');";

            var dataAccessMock = new Mock<IDataAccess>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.AddOrUpdateNgramsToTable(tableName, ngrams);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(commandText), Times.Once);
            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(commandText1), Times.Once);
        }

        [Theory]
        [InlineData("tableName")]
        public void AddOrUpdateNgramsToTable_EmptyNgrams_Verify(string tableName)
        {
            var ngrams = new List<NGram>();
            var dataAccessMock = new Mock<IDataAccess>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.AddOrUpdateNgramsToTable(tableName, ngrams);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("tableName")]
        public void AddOrUpdateNgramsToTable_NullNgrams_Verify(string tableName)
        {
            var dataAccessMock = new Mock<IDataAccess>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.AddOrUpdateNgramsToTable(tableName, null);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("tableName")]
        public void AddNgramsToTable_Unigrams_Verify(string tableName)
        {
            var ngrams = new List<NGram>
            {
                new NGram {Value = 10, WordsList = new List<string> {"small"}},
                new NGram {Value = 10, WordsList = new List<string> {"big"}}
            };

            var commandText = "INSERT INTO `" + tableName +
                              "` (`Value`, `Word1`) VALUES('" + ngrams[0].Value +
                              "', '" + ngrams[0].WordsList[0] +
                              "'),('" + ngrams[1].Value + "', '" + ngrams[1].WordsList[0] +
                              "');";

            var dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(m => m.ExecuteNonQueryDb(commandText)).Verifiable();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.AddNgramsToTable(tableName, ngrams);

            dataAccessMock.Verify();
        }

        [Theory]
        [InlineData("tableName")]
        public void AddNgramsToTable_EmptyNgrams_Verify(string tableName)
        {
            var ngrams = new List<NGram>();
            var dataAccessMock = new Mock<IDataAccess>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.AddNgramsToTable(tableName, ngrams);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("tableName")]
        public void AddNgramsToTable_NullNgrams_Verify(string tableName)
        {
            var dataAccessMock = new Mock<IDataAccess>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object);
            creator.AddNgramsToTable(tableName, null);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(It.IsAny<string>()), Times.Never);
        }
    }
}

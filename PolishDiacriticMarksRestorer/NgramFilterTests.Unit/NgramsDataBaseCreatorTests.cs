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
            const string commandText = "a";

            var dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(m => m.ExecuteNonQueryServer(commandText)).Verifiable();

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.CreateDbString(name)).Returns(commandText);

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
            creator.CreateDataBase(name);

            dataAccessMock.Verify();
        }

        [Theory]
        [InlineData("dbName", "tableName")]
        public void CreateTable_Verify(string dbName, string tableName)
        {
            const string commandText = "b";
            const string commandText2 = "c";

            var dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(m => m.ExecuteNonQueryServer(commandText)).Verifiable();
            dataAccessMock.Setup(m => m.ExecuteNonQueryServer(commandText2)).Verifiable();

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.CreateNgramsTableString(dbName, tableName, 1)).Returns(commandText);
            queryProviderMock.Setup(m => m.CreateAddProcedureString(dbName, tableName, 1)).Returns(commandText2);

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
            creator.CreateTables(dbName, tableName, 1);

            dataAccessMock.Verify(m => m.ExecuteNonQueryServer(It.IsAny<string>()), Times.Exactly(2));
        }

        [Theory]
        [InlineData("dbName", "tableName")]
        public void CreateTable_NotCreate_Verify(string dbName, string tableName)
        {
            var dataAccessMock = new Mock<IDataAccess>();

            var queryProviderMock = new Mock<IQueryProvider>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
            creator.CreateTables(dbName, tableName, -1);

            dataAccessMock.Verify(m => m.ExecuteNonQueryServer(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("tableName")]
        public void AddNgramsToTable_Digrams_Verify(string tableName)
        {
            var ngrams = new List<NGram>
            {
                new NGram (10, new List<string> {"small", "cat"}),
                new NGram (10, new List<string> {"big", "cat"})
            };

            const string commandText = "ss";

            var dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(m => m.ExecuteNonQueryDb(commandText)).Verifiable();

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.InsertNgramsString(tableName, ngrams)).Returns(commandText);

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
            creator.AddNgramsToTable(tableName, ngrams);

            dataAccessMock.Verify();
        }

        [Theory]
        [InlineData("tableName")]
        public void AddNgramsToTable_EmptyNgrams_Verify(string tableName)
        {
            var ngrams = new List<NGram>();
            var dataAccessMock = new Mock<IDataAccess>();

            var queryProviderMock = new Mock<IQueryProvider>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
            creator.AddNgramsToTable(tableName, ngrams);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("tableName")]
        public void AddNgramsToTable_NullNgrams_Verify(string tableName)
        {
            var dataAccessMock = new Mock<IDataAccess>();

            var queryProviderMock = new Mock<IQueryProvider>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
            creator.AddNgramsToTable(tableName, null);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("tableName")]
        public void AddOrUpdateNgramsToTable_Digrams_Verify(string tableName)
        {
            var ngrams = new List<NGram>
            {
                new NGram (10, new List<string> {"small", "cat"}),
                new NGram (10, new List<string> {"big", "cat"})
            };

            var commandText = "aa";
            var commandText1 = "bb";

            var dataAccessMock = new Mock<IDataAccess>();

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.InsertOrUpdateNgramString(ngrams[0])).Returns(commandText);
            queryProviderMock.Setup(m => m.InsertOrUpdateNgramString(ngrams[1])).Returns(commandText1);

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
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

            var queryProviderMock = new Mock<IQueryProvider>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
            creator.AddOrUpdateNgramsToTable(tableName, ngrams);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("tableName")]
        public void AddOrUpdateNgramsToTable_NullNgrams_Verify(string tableName)
        {
            var dataAccessMock = new Mock<IDataAccess>();

            var queryProviderMock = new Mock<IQueryProvider>();

            var creator = new NgramsDataBaseCreator(dataAccessMock.Object, queryProviderMock.Object);
            creator.AddOrUpdateNgramsToTable(tableName, null);

            dataAccessMock.Verify(m => m.ExecuteNonQueryDb(It.IsAny<string>()), Times.Never);
        }
    }
}

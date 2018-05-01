using System.Data;
using Moq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class DataBaseManagerTests
    {
        [Fact]
        public void ConnectToServer_Verify()
        {
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.Open()).Verifiable();

            var connectionFactoryMock = new Mock<IDataBaseManagerFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnectionServer(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            var sut = new DataBaseManager(connectionFactoryMock.Object, "", "", "", "");
            sut.ConnectToServer();
            sut.Disconnect();

            connectionMock.Verify();
        }

        [Fact]
        public void ConnectToDb_Verify()
        {
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.Open()).Verifiable();

            var connectionFactoryMock = new Mock<IDataBaseManagerFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnectionDb(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            var sut = new DataBaseManager(connectionFactoryMock.Object, "", "", "", "");
            sut.ConnectToDb();
            sut.Disconnect();

            connectionMock.Verify();
        }

        [Fact]
        public void Disconnect_Verify()
        {
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.Close()).Verifiable();

            var connectionFactoryMock = new Mock<IDataBaseManagerFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnectionDb(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            connectionFactoryMock
                .Setup(m => m.CreateConnectionServer(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            var sut = new DataBaseManager(connectionFactoryMock.Object, "", "", "", "");
            sut.ConnectToDb();
            sut.ConnectToServer();
            sut.Disconnect();

            connectionMock.Verify();
        }

        [Fact]
        public void ExecuteSqlCommand_NotEmptyDataSetTables()
        {
            var commandMock = new Mock<IDbCommand>();
            commandMock
                .Setup(m => m.ExecuteNonQuery())
                .Verifiable();

            var connectionMock = new Mock<IDbConnection>();
            connectionMock
                .Setup(m => m.CreateCommand())
                .Returns(commandMock.Object);

            var dataAdapterMock = new Mock<IDataAdapter>();
            dataAdapterMock
                .Setup(m => m.Fill(It.IsAny<DataSet>()))
                .Callback((DataSet myval) => { myval.Tables.Add(new DataTable()); });

            var connectionFactoryMock = new Mock<IDataBaseManagerFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateDataAdapter(It.IsAny<string>()))
                .Returns(dataAdapterMock.Object);
            connectionFactoryMock
                .Setup(m => m.CreateConnectionDb(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            DataSet result;
            using (var sut = new DataBaseManager(connectionFactoryMock.Object, "", "", "", ""))
            {
                sut.ConnectToDb();
                result = sut.ExecuteSqlCommand("");
            }

            Assert.NotEmpty(result.Tables);
        }

        [Theory]
        [InlineData("test")]
        public void ExecuteNonQueryServer_Verify(string text)
        {
            var commandMock = new Mock<IDbCommand>();
            commandMock
                .Setup(m => m.ExecuteNonQuery())
                .Verifiable();
            commandMock.SetupProperty(m => m.CommandText);

            var connectionMock = new Mock<IDbConnection>();
            connectionMock
                .Setup(m => m.CreateCommand())
                .Returns(commandMock.Object);

            var connectionFactoryMock = new Mock<IDataBaseManagerFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnectionServer(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            using (var sut = new DataBaseManager(connectionFactoryMock.Object, "", "", "", ""))
            {
                sut.ConnectToServer();
                sut.ExecuteNonQueryServer(text);
            }

            commandMock.Verify();
            Assert.Equal(text, commandMock.Object.CommandText);
        }

        [Theory]
        [InlineData("test")]
        public void ExecuteNonQueryDb_Verify(string text)
        {
            var commandMock = new Mock<IDbCommand>();
            commandMock
                .Setup(m => m.ExecuteNonQuery())
                .Verifiable();
            commandMock.SetupProperty(m => m.CommandText);

            var connectionMock = new Mock<IDbConnection>();
            connectionMock
                .Setup(m => m.CreateCommand())
                .Returns(commandMock.Object);

            var connectionFactoryMock = new Mock<IDataBaseManagerFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnectionDb(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            using (var sut = new DataBaseManager(connectionFactoryMock.Object, "", "", "", ""))
            {
                sut.ConnectToDb();
                sut.ExecuteNonQueryDb(text);
            }

            commandMock.Verify();
            Assert.Equal(text, commandMock.Object.CommandText);
        }

    }
}

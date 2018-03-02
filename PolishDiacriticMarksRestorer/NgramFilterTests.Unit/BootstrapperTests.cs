using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using Xunit;
using NgramFilter;
using NgramFilter.Interfaces;

namespace NgramFilterTests.Unit
{
    public class BootstrapperTests
    {
        [Fact]
        public void Filter_OutputExistsWithContent()
        {
            var filterMock = new Mock<IFilter>();
            filterMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns(true);
            var modifierMock = new Mock<IModifier>();
            modifierMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns((NGram myval) => myval);
            var dbMock = new Mock<IDataBaseManagerFactory>();
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\input", new MockFileData(@"15 small cat") }
            });
            var queryProvider = new Mock<IQueryProvider>();

            var bootstrapper = new Bootstrapper(filterMock.Object, modifierMock.Object, mockFileSystem, dbMock.Object,queryProvider.Object);
            bootstrapper.Filter(@"C:\input", @"C:\output");

            Assert.True(mockFileSystem.FileExists(@"C:\output"));
            Assert.Equal("15 small cat\r\n", mockFileSystem.File.ReadAllText(@"C:\output"));
        }

        [Fact]
        public void Filter_OutputExistsWithoutContent()
        {
            var filterMock = new Mock<IFilter>();
            filterMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns(false);
            var modifierMock = new Mock<IModifier>();
            modifierMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns((NGram myval) => myval);
            var dbMock = new Mock<IDataBaseManagerFactory>();
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\input", new MockFileData(@"15 small cat") }
            });
            var queryProvider = new Mock<IQueryProvider>();

            var bootstrapper = new Bootstrapper(filterMock.Object, modifierMock.Object, mockFileSystem, dbMock.Object, queryProvider.Object);
            bootstrapper.Filter(@"C:\input", @"C:\output");

            Assert.True(mockFileSystem.FileExists(@"C:\output"));
            Assert.NotEqual("15 small cat", mockFileSystem.File.ReadAllText(@"C:\output"));
        }

        [Fact]
        public void Filter_ThrowFileNotFoundException()
        {
            var filterMock = new Mock<IFilter>();
            filterMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns(false);
            var modifierMock = new Mock<IModifier>();
            modifierMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns((NGram myval) => myval);
            var dbMock = new Mock<IDataBaseManagerFactory>();
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var queryProvider = new Mock<IQueryProvider>();

            var bootstrapper = new Bootstrapper(filterMock.Object, modifierMock.Object, mockFileSystem, dbMock.Object, queryProvider.Object);

            var exception = Assert.Throws<FileNotFoundException>(() => bootstrapper.Filter(@"C:\input", @"C:\output"));
            Assert.IsType<FileNotFoundException>(exception);
        }

        [Fact]
        public void Filter_ThrowFormatException()
        {
            var filterMock = new Mock<IFilter>();
            filterMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns(false);
            var modifierMock = new Mock<IModifier>();
            modifierMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns((NGram myval) => myval);
            var dbMock = new Mock<IDataBaseManagerFactory>();
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\input", new MockFileData(@"aa small cat") }
            });
            var queryProvider = new Mock<IQueryProvider>();

            var bootstrapper = new Bootstrapper(filterMock.Object, modifierMock.Object, mockFileSystem, dbMock.Object, queryProvider.Object);

            var exception = Assert.Throws<FormatException>(() => bootstrapper.Filter(@"C:\input", @"C:\output"));
            Assert.IsType<FormatException>(exception);
        }

        [Fact]
        public void CreateDb_WithoutModifierItem_Verify()
        {
            var filterMock = new Mock<IFilter>();
            var modifierMock = new Mock<IModifier>();
            modifierMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns((NGram myval) => myval);

            var commandMock = new Mock<IDbCommand>();
            commandMock
                .Setup(m => m.ExecuteNonQuery())
                .Verifiable();

            var connectionMock = new Mock<IDbConnection>();
            connectionMock
                .Setup(m => m.CreateCommand())
                .Returns(commandMock.Object);

            var connectionFactoryMock = new Mock<IDataBaseManagerFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnectionServer(It.IsAny<string>()))
                .Returns(connectionMock.Object);
            connectionFactoryMock
                .Setup(m => m.CreateConnectionDb(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\input", new MockFileData(@"15 small cat") }
            });
            var queryProvider = new Mock<IQueryProvider>();

            var bootstrapper = new Bootstrapper(filterMock.Object, modifierMock.Object, mockFileSystem, connectionFactoryMock.Object, queryProvider.Object);
            bootstrapper.CreateDb(@"C:\input", "dbName", "tableName");

            commandMock.Verify();
        }

        [Fact]
        public void CreateDb_WithModifierItem_Verify()
        {
            var filterMock = new Mock<IFilter>();
            var modifierMock = new Mock<IModifier>();
            modifierMock.Setup(m => m.Start(It.IsAny<NGram>())).Returns((NGram myval) => myval);
            modifierMock.Setup(m => m.Size()).Returns(1);

            var commandMock = new Mock<IDbCommand>();
            commandMock
                .Setup(m => m.ExecuteNonQuery())
                .Verifiable();

            var connectionMock = new Mock<IDbConnection>();
            connectionMock
                .Setup(m => m.CreateCommand())
                .Returns(commandMock.Object);

            var connectionFactoryMock = new Mock<IDataBaseManagerFactory>();
            connectionFactoryMock
                .Setup(m => m.CreateConnectionServer(It.IsAny<string>()))
                .Returns(connectionMock.Object);
            connectionFactoryMock
                .Setup(m => m.CreateConnectionDb(It.IsAny<string>()))
                .Returns(connectionMock.Object);

            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\input", new MockFileData(@"15 small cat") }
            });
            var queryProvider = new Mock<IQueryProvider>();

            var bootstrapper = new Bootstrapper(filterMock.Object, modifierMock.Object, mockFileSystem, connectionFactoryMock.Object, queryProvider.Object);
            bootstrapper.CreateDb(@"C:\input", "dbName", "tableName");

            commandMock.Verify();
        }
    }
}

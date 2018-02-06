using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class FileManagerTests
    {
        [Fact]
        public void Open_OpenFileRead_True()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            var opened = file.Open(FileManagerType.Read);
            file.Close();

            Assert.True(opened);
        }

        [Fact]
        public void Open_OpenFileWrite_True()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            var opened = file.Open(FileManagerType.Write);
            file.Close();

            Assert.True(opened);
        }

        [Fact]
        public void Open_OpenFileNothing_False()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            var opened = file.Open(FileManagerType.Nothing);
            file.Close();

            Assert.False(opened);
        }

        [Fact]
        public void Open_OpenNotExistFile_False()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            var opened = file.Open(FileManagerType.Nothing);
            file.Close();

            Assert.False(opened);
        }

        [Fact]
        public void ReadLineFromFile_Equal()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            file.Open(FileManagerType.Read);
            var result = file.ReadLine();
            file.Close();

            Assert.Equal(@"cat",result);
        }

        [Fact]
        public void ReadLineFromFile_NotEqual()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            file.Open(FileManagerType.Read);
            var result = file.ReadLine();
            file.Close();

            Assert.NotEqual(@"cat1", result);
        }

        [Fact]
        public void ReadLineFromOpenToWriteFile_Null()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            file.Open(FileManagerType.Write);
            var result = file.ReadLine();
            file.Close();

            Assert.Null(result);
        }

        [Fact]
        public void WriteLineToFile_Equal()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            file.Open(FileManagerType.Write);
            file.WriteLine(@"cat");
            file.Close();

            file.Open(FileManagerType.Read);
            var result = file.ReadLine();
            file.Close();

            Assert.Equal(@"cat", result);
        }

        [Fact]
        public void CountLinesInFile_Equal()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData("small\r\ncat") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            var result = file.CountLines();

            Assert.Equal(2, result);
        }

        [Fact]
        public void CreateFile_True()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            file.Create();
            var result = mockFileSystem.File.Exists(@"C:\test1");

            Assert.True(result);
        }

        [Fact]
        public void CreateFile_False()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            file.Create();
            var result = mockFileSystem.File.Exists(@"C:\test2");

            Assert.False(result);
        }
    }
}

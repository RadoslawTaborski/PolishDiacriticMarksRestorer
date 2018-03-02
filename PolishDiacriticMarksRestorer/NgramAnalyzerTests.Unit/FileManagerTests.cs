using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using System.IO.Abstractions.TestingHelpers;
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

            bool opened;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                opened = file.Open(FileManagerType.Read);
            }

            Assert.True(opened);
        }

        [Fact]
        public void Open_OpenFileWrite_True()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            bool opened;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                opened = file.Open(FileManagerType.Write);
            }

            Assert.True(opened);
        }

        [Fact]
        public void Open_OpenFileNothing_False()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            bool opened;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                opened = file.Open(FileManagerType.Nothing);
            }

            Assert.False(opened);
        }

        [Fact]
        public void Open_OpenNotExistFile_False()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            bool opened;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                opened = file.Open(FileManagerType.Nothing);
            }

            Assert.False(opened);
        }

        [Fact]
        public void ReadLineFromFile_Equal()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            string result;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                file.Open(FileManagerType.Read);
                result = file.ReadLine();
            }

            Assert.Equal(@"cat",result);
        }

        [Fact]
        public void ReadLineFromFile_NotEqual()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            string result;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                file.Open(FileManagerType.Read);
                result = file.ReadLine();
            }

            Assert.NotEqual(@"cat1", result);
        }

        [Fact]
        public void ReadLineFromOpenToWriteFile_Null()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            string result;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                file.Open(FileManagerType.Write);
                result = file.ReadLine();
            }

            Assert.Null(result);
        }

        [Fact]
        public void WriteLineToFile_Equal()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"") }
            });

            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                file.Open(FileManagerType.Write);
                file.WriteLine(@"cat");
            }

            string result;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                file.Open(FileManagerType.Read);
                result = file.ReadLine();
            }

            Assert.Equal(@"cat", result);
        }

        [Fact]
        public void CountLinesInFile_Equal()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData("small\r\ncat") }
            });

            int result;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                result = file.CountLines();
            }

            Assert.Equal(2, result);
        }

        [Fact]
        public void CreateFile_True()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            bool result;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                file.Create();
                result = mockFileSystem.File.Exists(@"C:\test1");
            }

            Assert.True(result);
        }

        [Fact]
        public void CreateFile_False()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            bool result;
            using (IFileAccess file = new FileManager(mockFileSystem, @"C:\test1"))
            {
                file.Create();
                result = mockFileSystem.File.Exists(@"C:\test2");
            }

            Assert.False(result);
        }
    }
}

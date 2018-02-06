using System.Collections.Generic;
using System.IO;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class FileManagerTests
    {
        [Fact]
        public void OpenAndCloseFile()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\test1", new MockFileData(@"cat") }
            });

            IFileAccess file = new FileManager(mockFileSystem, @"C:\test1");

            var opened = file.Open(FileManagerType.Read);
            var closed = file.Close();

            Assert.True(opened);
            Assert.True(closed);
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
    }
}

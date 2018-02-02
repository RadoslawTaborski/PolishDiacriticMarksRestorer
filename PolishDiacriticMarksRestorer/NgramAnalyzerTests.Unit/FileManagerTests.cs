using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class FileManagerTests
    {
        [Fact]
        public void OpenAndCloseFile()
        {
            IFileAccess file = new FileManager("E:\\PWr\\magisterskie\\magisterka\\Projekt\\PolishDiacriticMarksRestorer\\NgramAnalyzerTests.Unit/test.txt");

            var opened = file.Open(FileManagerType.Read);
            var closed = file.Close();

            Assert.True(opened);
            Assert.True(closed);
        }

        [Fact]
        public void ReadLineFromFile()
        {
            IFileAccess file = new FileManager("E:\\PWr\\magisterskie\\magisterka\\Projekt\\PolishDiacriticMarksRestorer\\NgramAnalyzerTests.Unit/test.txt");

            file.Open(FileManagerType.Read);
            var result = file.ReadLine();
            file.Close();

            Assert.Equal("kot",result);
        }
    }
}

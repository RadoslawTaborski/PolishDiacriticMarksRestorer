using System.Data;
using Moq;
using Xunit;
using NgramAnalyzer;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzerTests.Unit
{
    public class AnalyzerTests
    {
        [Fact]
        public void AnalyzeStrings_ReturnsStringArray()
        {
            var tab = new DataTable();
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Rows.Add(25, "cat");
            tab.Rows.Add(15, "dog");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand(It.IsAny<string>())).Returns(ds);

            var analyze = new Analyzer();
        analyze.SetData(dataMock.Object);
            var result = analyze.AnalyzeStrings(It.IsAny<string[]>());

        Assert.Equal(new[]{"25","cat"},result);
        }
    }
}

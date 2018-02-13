using System.Collections.Generic;
using System.Data;
using Moq;
using Xunit;
using NgramAnalyzer;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzerTests.Unit
{
    public class AnalyzerTests
    {
        [Fact]
        public void AnalyzeStrings_ReturnsStringArray()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Rows.Add(1,25, "cat");
            tab.Rows.Add(2,15, "dog");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand(It.IsAny<string>())).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<string>>())).Returns("aa");

            var analyze = new Analyzer();
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new []{"a"});

            Assert.Equal(new[] { "25", "cat", "\r\n", "15", "dog", "\r\n" }, result);
        }
    }
}

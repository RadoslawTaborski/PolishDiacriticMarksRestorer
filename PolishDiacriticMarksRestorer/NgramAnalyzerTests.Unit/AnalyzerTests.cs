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
        public void AnalyzeStrings_Find1ElementUnigram()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1,25, "za", "przyjêciem");
            tab.Rows.Add(2,15, "za", "przyjêciem,");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(int));
            tab2.Columns.Add("Word1", typeof(string));
            tab2.Rows.Add(1, 25, "przyjêciem");
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.GetMultiNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<string>>(), It.IsAny<List<string>>())).Returns("di");
            queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");

            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();
            diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjêciem", 1)
            });

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string>{"za", "przyjeciem"});

            Assert.Equal(new List<string>{"za", "przyjêciem"}, result);
        }

        [Fact]
        public void AnalyzeStrings_Find2ElementsUnigramAndDigram()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1, 25, "za", "przyjêciem");
            tab.Rows.Add(2, 15, "za", "przyjêciem,");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(int));
            tab2.Columns.Add("Word1", typeof(string));
            tab2.Rows.Add(1, 25, "przyjêciem");
            tab2.Rows.Add(1, 25, "przyjêciem,");
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.GetMultiNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<string>>(), It.IsAny<List<string>>())).Returns("di");
            queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");

            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();
            diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjêciem", 1),
                new KeyValuePair<string, int>("przyjêciem,", 2)
            });

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem" });

            Assert.Equal(new List<string> { "za", "przyjêciem" }, result);
        }

        [Fact]
        public void AnalyzeStrings_Find1ElementDigram()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1, 25, "za", "przyjêciem");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(int));
            tab2.Columns.Add("Word1", typeof(string));
            tab2.Rows.Add(1, 25, "przyjêciem");
            tab2.Rows.Add(1, 25, "przyjêciem,");
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.GetMultiNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<string>>(), It.IsAny<List<string>>())).Returns("di");
            queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");

            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();
            diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjêciem", 1),
                new KeyValuePair<string, int>("przyjêciem,", 2)
            });

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem" });

            Assert.Equal(new List<string> { "za", "przyjêciem" }, result);
        }

        [Fact]
        public void AnalyzeStrings_IntTryParse_False()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1, 25, "za", "przyjêciem");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(string));
            tab2.Columns.Add("Word1", typeof(string));
            tab2.Rows.Add(1, "aa", "przyjêciem");
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.GetMultiNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<string>>(), It.IsAny<List<string>>())).Returns("di");
            queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");

            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();
            diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjêciem", 1),
                new KeyValuePair<string, int>("przyjêciem,", 2)
            });

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem" });

            Assert.Equal(new List<string> { "za", "przyjeciem" }, result);
        }
    }
}

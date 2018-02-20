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
        public void AnalyzeStrings_Digram_Analyze2Words()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1,25, "za", "przyj�ciem");
            tab.Rows.Add(2,15, "za", "przyjeciem,");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(int));
            tab2.Columns.Add("Word1", typeof(string));
            tab2.Rows.Add(1, 25, "przyj�ciem");
            tab2.Rows.Add(2, 56, "za");
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");
            queryProviderMock.Setup(m => m.GetAllNecessaryNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<List<List<string>>>>())).Returns("di");

            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();
            diacriticAdderMock.Setup(m => m.Start("za", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("za", 0),
                new KeyValuePair<string, int>("z�", 1)
            });
            diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjeciem", 0),
                new KeyValuePair<string, int>("przyj�ciem", 1)
            });

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string>{"za", "przyjeciem"});

            Assert.Equal(new List<string>{"za", "przyj�ciem"}, result);
        }

        [Fact]
        public void AnalyzeStrings_DigramAnalyze3Words()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1, 25, "za", "przyj�ciem");
            tab.Rows.Add(2, 15, "za", "przyjeciem,");
            tab.Rows.Add(2, 56, "przyj�ciem", "uchwa�y,");
            tab.Rows.Add(2, 56, "przyj�ciem", "uchwaly,");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(int));
            tab2.Columns.Add("Word1", typeof(string));
            tab2.Rows.Add(1, 25, "przyj�ciem");
            tab2.Rows.Add(2, 56, "za");
            tab2.Rows.Add(3, 28, "uchwa�y");
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");
            queryProviderMock.Setup(m => m.GetAllNecessaryNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<List<List<string>>>>())).Returns("di");

            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();
            diacriticAdderMock.Setup(m => m.Start("za", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("za", 0),
                new KeyValuePair<string, int>("z�", 1)
            });
            diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjeciem", 0),
                new KeyValuePair<string, int>("przyj�ciem", 1)
            });
            diacriticAdderMock.Setup(m => m.Start("uchwaly", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("uchwaly", 0),
                new KeyValuePair<string, int>("uchwa�y", 1),
                new KeyValuePair<string, int>("uchw�ly", 1),
            });

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "uchwaly" });

            Assert.Equal(new List<string> { "za", "przyj�ciem", "uchwa�y" }, result);
        }

        [Fact]
        public void AnalyzeStrings_NgramVariantsCount0()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1, 25, "za", "przyj�ciem");
            tab.Rows.Add(2, 15, "za", "przyjeciem,");
            tab.Rows.Add(2, 56, "przyj�ciem", "uchwa�y,");
            tab.Rows.Add(2, 56, "przyj�ciem", "uchwaly,");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(int));
            tab2.Columns.Add("Word1", typeof(string));
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");
            queryProviderMock.Setup(m => m.GetAllNecessaryNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<List<List<string>>>>())).Returns("di");

            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();
            diacriticAdderMock.Setup(m => m.Start("za", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("za", 0),
                new KeyValuePair<string, int>("z�", 1)
            });
            diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjeciem", 0),
                new KeyValuePair<string, int>("przyj�ciem", 1)
            });
            diacriticAdderMock.Setup(m => m.Start("uchwaly", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("uchwaly", 0),
                new KeyValuePair<string, int>("uchwa�y", 1),
                new KeyValuePair<string, int>("uchw�ly", 1),
            });

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "uchwaly" });

            Assert.Equal(new List<string> { "za", "przyjeciem", "uchwaly" }, result);
        }

        [Fact]
        public void AnalyzeStrings_TryParse_False()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(string));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1, "aa", "za", "przyj�ciem");
            var ds = new DataSet();
            ds.Tables.Add(tab);

            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(int));
            tab2.Columns.Add("Word1", typeof(string));
            tab2.Rows.Add(1, 25, "przyj�ciem");
            tab2.Rows.Add(2, 56, "za");
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(ds);

            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");
            queryProviderMock.Setup(m => m.GetAllNecessaryNgramsFromTable(It.IsAny<NgramType>(), It.IsAny<List<List<List<string>>>>())).Returns("di");

            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();
            diacriticAdderMock.Setup(m => m.Start("za", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("za", 0),
                new KeyValuePair<string, int>("z�", 1)
            });
            diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjeciem", 0),
                new KeyValuePair<string, int>("przyj�ciem", 1)
            });

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem"});

            Assert.Equal(new List<string> { "za", "przyj�ciem"}, result);
        }
    }
}

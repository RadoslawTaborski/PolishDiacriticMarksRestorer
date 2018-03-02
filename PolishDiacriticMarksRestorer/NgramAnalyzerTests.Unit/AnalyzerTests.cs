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
        private readonly DataSet _unigrams=new DataSet();
        private readonly DataSet _digrams = new DataSet();
        private readonly DataSet _trigrams = new DataSet();
        private readonly DataSet _fourgrams = new DataSet();
        private readonly Mock<IDataAccess> _dataMock= new Mock<IDataAccess>();
        private readonly Mock<IQueryProvider> _queryProviderMock = new Mock<IQueryProvider>();
        private readonly Mock<IDiacriticMarksAdder> _diacriticAdderMock = new Mock<IDiacriticMarksAdder>();

        public AnalyzerTests()
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Rows.Add(1, 25, "przyj�ciem");
            tab.Rows.Add(2, 21, "uchwa�y");
            tab.Rows.Add(3, 56, "za");
            tab.Rows.Add(10, 56, "nowej");
            _unigrams.Tables.Add(tab);

            tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Rows.Add(1, 25, "za", "przyj�ciem");
            tab.Rows.Add(2, 15, "za", "przyjeciem,");
            _digrams.Tables.Add(tab);

            tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Columns.Add("Word3", typeof(string));
            tab.Rows.Add(1, 25, "za", "przyj�ciem", "uchwa�y");
            tab.Rows.Add(2, 15, "za", "przyjeciem,", "uchwa�y");
            tab.Rows.Add(3, 45, "przyj�ciem", "nowej", "uchwa�y");
            tab.Rows.Add(4, 17, "przyjeciem", "nowej,", "uchwa�y");
            tab.Rows.Add(5, 50, "nowej", "uchwa�y", "z");
            tab.Rows.Add(6, 17, "nowej", "uchwaly", "z");
            _trigrams.Tables.Add(tab);

            tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("Value", typeof(int));
            tab.Columns.Add("Word1", typeof(string));
            tab.Columns.Add("Word2", typeof(string));
            tab.Columns.Add("Word3", typeof(string));
            tab.Columns.Add("Word4", typeof(string));
            tab.Rows.Add(1, 25, "za", "przyj�ciem", "nowej","uchwa�y");
            tab.Rows.Add(2, 15, "za", "przyjeciem", "nowej", "uchwa�y");
            tab.Rows.Add(1, 27, "przyj�ciem", "nowej", "uchwa�y", "z");
            tab.Rows.Add(2, 12, "przyjeciem", "nowej", "uchwa�y", "z");
            _fourgrams.Tables.Add(tab);

            _dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(_unigrams);
            _dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(_digrams);
            _dataMock.Setup(m => m.ExecuteSqlCommand("tri")).Returns(_trigrams);
            _dataMock.Setup(m => m.ExecuteSqlCommand("four")).Returns(_fourgrams);

            _queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni");
            _queryProviderMock.Setup(m => m.GetAllNecessaryNgramsFromTable(NgramType.Digram, It.IsAny<List<List<List<string>>>>())).Returns("di");
            _queryProviderMock.Setup(m => m.GetAllNecessaryNgramsFromTable(NgramType.Trigram, It.IsAny<List<List<List<string>>>>())).Returns("tri");
            _queryProviderMock.Setup(m => m.GetAllNecessaryNgramsFromTable(NgramType.Fourgram, It.IsAny<List<List<List<string>>>>())).Returns("four");

            _diacriticAdderMock.Setup(m => m.Start("za", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("za", 0),
                new KeyValuePair<string, int>("z�", 1)
            });
            _diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjeciem", 0),
                new KeyValuePair<string, int>("przyj�ciem", 1)
            });
            _diacriticAdderMock.Setup(m => m.Start("uchwaly", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("uchwaly", 0),
                new KeyValuePair<string, int>("uchwa�y", 1)
            });
            _diacriticAdderMock.Setup(m => m.Start("nowej", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("nowej", 0),
                new KeyValuePair<string, int>("n�wej", 1)
            });
            _diacriticAdderMock.Setup(m => m.Start("z", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("z", 0),
                new KeyValuePair<string, int>("�", 1)
            });
        }

        [Fact]
        public void AnalyzeStrings_Digram_Only2Words()
        {
            var analyze = new Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string>{"za", "przyjeciem"});

            Assert.Equal(new List<string>{"za", "przyj�ciem"}, result);
        }

        [Fact]
        public void AnalyzeStrings_Trigram_Only3Words()
        {
            var analyze = new Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Trigram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "uchwaly" });

            Assert.Equal(new List<string> { "za", "przyj�ciem", "uchwa�y" }, result);
        }

        [Fact]
        public void AnalyzeStrings_Trigram_Only4Words()
        {
            var analyze = new Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Trigram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "nowej", "uchwaly" });

            Assert.Equal(new List<string> { "za", "przyj�ciem", "nowej", "uchwa�y" }, result);
        }

        [Fact]
        public void AnalyzeStrings_Trigram_Only5Words()
        {
            var analyze = new Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Trigram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "nowej", "uchwaly", "z" });

            Assert.Equal(new List<string> { "za", "przyj�ciem", "nowej", "uchwa�y", "z" }, result);
        }

        [Fact]
        public void AnalyzeStrings_Fourgrams_Only4Words()
        {
            var analyze = new Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Fourgram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "nowej", "uchwaly"});

            Assert.Equal(new List<string> { "za", "przyj�ciem", "nowej", "uchwa�y"}, result);
        }

        [Fact]
        public void AnalyzeStrings_Fourgrams_Only5Words()
        {
            var analyze = new Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Fourgram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "nowej", "uchwaly", "z" });

            Assert.Equal(new List<string> { "za", "przyj�ciem", "nowej", "uchwa�y", "z" }, result);
        }

        [Fact]
        public void AnalyzeStrings_ToShortText()
        {
            var queryProviderMock = new Mock<IQueryProvider>();
            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();

            var analyze = new Analyzer(diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za"});

            Assert.Equal(new List<string> { "za" }, result);
        }

        [Fact]
        public void AnalyzeStrings_DigramAnalyze3Words_FileDictionary()
        {
            var dictionaryMock = new Mock<IDictionary>();
            dictionaryMock.Setup(m => m.CheckWords(new List<string>{
                "za",
                "z�",
                "przyjeciem",
                "przyj�ciem",
                "uchwaly",
                "uchwa�y"
            })).Returns(new List<string> {"za", "przyj�ciem", "uchwa�y"});

            var analyze = new Analyzer(_diacriticAdderMock.Object, dictionaryMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "uchwaly" });

            Assert.Equal(new List<string> { "za", "przyj�ciem", "uchwa�y" }, result);
        }

        [Fact]
        public void AnalyzeStrings_NgramVariantsCount0()
        {
            var tab2 = new DataTable();
            tab2.Columns.Add("ID", typeof(int));
            tab2.Columns.Add("Value", typeof(int));
            tab2.Columns.Add("Word1", typeof(string));
            var ds2 = new DataSet();
            ds2.Tables.Add(tab2);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di")).Returns(_digrams);

            var analyze = new Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem", "uchwaly" });

            Assert.Equal(new List<string> { "za", "przyjeciem", "uchwaly" }, result);
        }

        [Fact]
        public void AnalyzeStrings_TryParse_False()
        {
            var analyze = new Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Digram);
            var result = analyze.AnalyzeStrings(new List<string> { "za", "przyjeciem"});

            Assert.Equal(new List<string> { "za", "przyj�ciem"}, result);
        }
    }
}

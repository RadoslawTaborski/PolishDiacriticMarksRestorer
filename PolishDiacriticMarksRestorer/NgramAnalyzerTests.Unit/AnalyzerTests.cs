using System.Collections.Generic;
using System.Data;
using Moq;
using Xunit;
using NgramAnalyzer;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
//TODO: repair DataSet in constructor 
namespace NgramAnalyzerTests.Unit
{
    public class AnalyzerTests
    {
        private readonly DataSet _unigrams=new DataSet();
        private readonly Mock<IDataAccess> _dataMock= new Mock<IDataAccess>();
        private readonly Mock<IQueryProvider> _queryProviderMock = new Mock<IQueryProvider>();
        private readonly Mock<IDiacriticMarksAdder> _diacriticAdderMock = new Mock<IDiacriticMarksAdder>();

        public AnalyzerTests()
        {
            var uni1 = new DataTable();
            uni1.Columns.Add("ID", typeof(int));
            uni1.Columns.Add("Value", typeof(int));
            uni1.Columns.Add("Word1", typeof(string));
            uni1.Rows.Add(1, 25, "przyjêciem");
            uni1.Rows.Add(2, 21, "uchwa³y");
            uni1.Rows.Add(3, 56, "za");
            uni1.Rows.Add(10, 56, "nowej");
            _unigrams.Tables.Add(uni1);

            var di1 = new DataTable();
            di1.Columns.Add("ID", typeof(int));
            di1.Columns.Add("Value", typeof(int));
            di1.Columns.Add("Word1", typeof(string));
            di1.Columns.Add("Word2", typeof(string));
            var di2 = di1.Clone();
            var di3 = di1.Clone();
            var di4 = di1.Clone();
            di1.Rows.Add(1, 25, "za", "przyjêciem");
            di2.Rows.Add(2, 15, "za", "przyjeciem");
            di3.Rows.Add(3, 2, "przyjeciem", "uchwaly,");
            di4.Rows.Add(4, 5, "przyjêciem", "uchwa³y,");
            //_digrams.Tables.Add(tab1);

            var tri1 = new DataTable();
            tri1.Columns.Add("ID", typeof(int));
            tri1.Columns.Add("Value", typeof(int));
            tri1.Columns.Add("Word1", typeof(string));
            tri1.Columns.Add("Word2", typeof(string));
            tri1.Columns.Add("Word3", typeof(string));
            var tri2 = tri1.Clone();
            var tri3 = tri1.Clone();
            var tri4 = tri1.Clone();
            var tri5 = tri1.Clone();
            var tri6 = tri1.Clone();
            tri1.Rows.Add(1, 25, "za", "przyjêciem", "nowej");
            tri2.Rows.Add(2, 15, "za", "przyjeciem", "nowej");
            tri3.Rows.Add(3, 45, "przyjêciem", "nowej", "uchwa³y");
            tri4.Rows.Add(4, 17, "przyjeciem", "nowej,", "uchwa³y");
            tri5.Rows.Add(5, 50, "nowej", "uchwa³y", "z");
            tri6.Rows.Add(6, 17, "nowej", "uchwaly", "z");

            var four1 = new DataTable();
            four1.Columns.Add("ID", typeof(int));
            four1.Columns.Add("Value", typeof(int));
            four1.Columns.Add("Word1", typeof(string));
            four1.Columns.Add("Word2", typeof(string));
            four1.Columns.Add("Word3", typeof(string));
            four1.Columns.Add("Word4", typeof(string));
            var four2 = four1.Clone();
            var four3 = four1.Clone();
            var four4 = four1.Clone();
            four1.Rows.Add(1, 25, "za", "przyjêciem", "nowej","uchwa³y");
            four2.Rows.Add(2, 15, "za", "przyjeciem", "nowej", "uchwa³y");
            four3.Rows.Add(1, 27, "przyjêciem", "nowej", "uchwa³y", "z");
            four4.Rows.Add(2, 12, "przyjeciem", "nowej", "uchwa³y", "z");

            _dataMock.Setup(m => m.ExecuteSqlCommand("uni1")).Returns(_unigrams);
            _dataMock.Setup(m => m.ExecuteSqlCommand("di1")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(di1); return tmp;});
            _dataMock.Setup(m => m.ExecuteSqlCommand("di2")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(di2); return tmp;});
            _dataMock.Setup(m => m.ExecuteSqlCommand("di3")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(di3); return tmp; });
            _dataMock.Setup(m => m.ExecuteSqlCommand("di4")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(di4); return tmp; });
            _dataMock.Setup(m => m.ExecuteSqlCommand("tri1")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(tri1); return tmp;});
            _dataMock.Setup(m => m.ExecuteSqlCommand("tri2")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(tri2); return tmp;});
            _dataMock.Setup(m => m.ExecuteSqlCommand("tri3")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(tri3); return tmp;});
            _dataMock.Setup(m => m.ExecuteSqlCommand("tri4")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(tri4); return tmp;});
            _dataMock.Setup(m => m.ExecuteSqlCommand("tri5")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(tri5); return tmp;});
            _dataMock.Setup(m => m.ExecuteSqlCommand("tri6")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(tri6); return tmp;});
            _dataMock.Setup(m => m.ExecuteSqlCommand("four1")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(four1); return tmp; });
            _dataMock.Setup(m => m.ExecuteSqlCommand("four2")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(four2); return tmp; });
            _dataMock.Setup(m => m.ExecuteSqlCommand("four3")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(four3); return tmp; });
            _dataMock.Setup(m => m.ExecuteSqlCommand("four4")).Returns(() => { var tmp = new DataSet(); tmp.Tables.Add(four4); return tmp; });

            _queryProviderMock.Setup(m => m.CheckWordsInUnigramFromTable(It.IsAny<List<string>>())).Returns("uni1");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Bigram, new List<string>() {"za", "przyjêciem"})).Returns("di1");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Bigram, new List<string>() { "za", "przyjeciem" })).Returns("di2");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Bigram, new List<string>() { "przyjeciem", "uchwaly" })).Returns("di3");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Bigram, new List<string>() { "przyjêciem", "uchwa³y" })).Returns("di4");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Trigram, new List<string>() { "za", "przyjêciem", "nowej" })).Returns("tri1");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Trigram, new List<string>() { "za", "przyjeciem", "nowej" })).Returns("tri2");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Trigram, new List<string>() { "przyjêciem", "nowej", "uchwa³y" })).Returns("tri3");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Trigram, new List<string>() { "przyjeciem", "nowej", "uchwa³y" })).Returns("tri4");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Trigram, new List<string>() { "nowej", "uchwa³y", "z" })).Returns("tri5");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Trigram, new List<string>() { "nowej", "uchwaly", "z" })).Returns("tri6");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Quadrigram, new List<string>() { "za", "przyjêciem", "nowej", "uchwa³y" })).Returns("four1");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Quadrigram, new List<string>() { "za", "przyjeciem", "nowej", "uchwa³y" })).Returns("four2");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Quadrigram, new List<string>() { "przyjêciem", "nowej", "uchwa³y", "z" })).Returns("four3");
            _queryProviderMock.Setup(m => m.GetTheSameNgramsFromTable(NgramType.Quadrigram, new List<string>() { "przyjeciem", "nowej", "uchwa³y", "z" })).Returns("four4");

            _diacriticAdderMock.Setup(m => m.Start("za", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("za", 0),
                new KeyValuePair<string, int>("z¹", 1)
            });
            _diacriticAdderMock.Setup(m => m.Start("przyjeciem", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("przyjeciem", 0),
                new KeyValuePair<string, int>("przyjêciem", 1)
            });
            _diacriticAdderMock.Setup(m => m.Start("uchwaly", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("uchwaly", 0),
                new KeyValuePair<string, int>("uchwa³y", 1)
            });
            _diacriticAdderMock.Setup(m => m.Start("nowej", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("nowej", 0),
                new KeyValuePair<string, int>("nówej", 1)
            });
            _diacriticAdderMock.Setup(m => m.Start("z", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("z", 0),
                new KeyValuePair<string, int>("Ÿ", 1)
            });
        }

        [Fact]
        public void AnalyzeStrings_Digram_Only2Words()
        {
            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Bigram);
            var result = analyze.AnalyzeString("za przyjeciem");

            Assert.Equal(new List<string>{"za"," ","przyjêciem"}, result);
        }

        [Fact]
        public void AnalyzeStrings_Trigram_Only3Words()
        {
            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Trigram);
            var result = analyze.AnalyzeString("za przyjeciem nowej");

            Assert.Equal(new List<string> { "za"," ", "przyjêciem"," ", "nowej" }, result);
        }

        [Fact]
        public void AnalyzeStrings_Trigram_Only4Words()
        {
            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Trigram);
            var result = analyze.AnalyzeString("za przyjeciem nowej uchwaly");

            Assert.Equal(new List<string> { "za"," ", "przyjêciem"," ", "nowej"," ", "uchwa³y" }, result);
        }

        [Fact]
        public void AnalyzeStrings_Trigram_Only5Words()
        {
            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Trigram);
            var result = analyze.AnalyzeString("za przyjeciem nowej uchwaly z");

            Assert.Equal(new List<string> { "za"," ", "przyjêciem"," ", "nowej"," ", "uchwa³y"," ", "z" }, result);
        }

        [Fact]
        public void AnalyzeStrings_Fourgrams_Only4Words()
        {
            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Quadrigram);
            var result = analyze.AnalyzeString("za przyjeciem nowej uchwaly");

            Assert.Equal(new List<string> { "za"," ", "przyjêciem"," ", "nowej"," ", "uchwa³y"}, result);
        }

        [Fact]
        public void AnalyzeStrings_Fourgrams_Only5Words()
        {
            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Quadrigram);
            var result = analyze.AnalyzeString("za przyjeciem nowej uchwaly z");

            Assert.Equal(new List<string> { "za"," ", "przyjêciem"," ", "nowej"," ", "uchwa³y"," ", "z" }, result);
        }

        [Fact]
        public void AnalyzeStrings_ToShortText()
        {
            var queryProviderMock = new Mock<IQueryProvider>();
            var diacriticAdderMock = new Mock<IDiacriticMarksAdder>();

            var analyze = new NgramAnalyzer.Analyzer(diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(queryProviderMock.Object);
            analyze.SetNgram(NgramType.Bigram);
            var result = analyze.AnalyzeString("za");

            Assert.Equal(new List<string> { "za" }, result);
        }

        [Fact]
        public void AnalyzeStrings_DigramAnalyze3Words_FileDictionary()
        {
            var dictionaryMock = new Mock<IDictionary>();
            dictionaryMock.Setup(m => m.CheckWords(new List<string>{
                "za",
                "z¹",
                "przyjeciem",
                "przyjêciem",
                "uchwaly",
                "uchwa³y"
            })).Returns(new List<string> {"za", "przyjêciem", "uchwa³y"});

            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object, dictionaryMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Bigram);
            var result = analyze.AnalyzeString("za przyjeciem uchwaly");

            Assert.Equal(new List<string> { "za"," ", "przyjêciem"," ", "uchwa³y" }, result);
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

            var tab1 = new DataTable();
            tab1.Columns.Add("ID", typeof(int));
            tab1.Columns.Add("Value", typeof(int));
            tab1.Columns.Add("Word1", typeof(string));
            tab1.Columns.Add("Word2", typeof(string));
            var ds1 = new DataSet();
            ds1.Tables.Add(tab1);

            var dataMock = new Mock<IDataAccess>();
            dataMock.Setup(m => m.ExecuteSqlCommand("uni1")).Returns(ds2);
            dataMock.Setup(m => m.ExecuteSqlCommand("di1")).Returns(ds1);
            dataMock.Setup(m => m.ExecuteSqlCommand("di2")).Returns(ds1);
            dataMock.Setup(m => m.ExecuteSqlCommand("di3")).Returns(ds1);
            dataMock.Setup(m => m.ExecuteSqlCommand("di4")).Returns(ds1);

            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Bigram);
            var result = analyze.AnalyzeString("za przyjeciem uchwaly");

            Assert.Equal(new List<string> { "za"," ", "przyjeciem"," ", "uchwaly" }, result);
        }

        [Fact]
        public void AnalyzeStrings_TryParse_False()
        {
            var analyze = new NgramAnalyzer.Analyzer(_diacriticAdderMock.Object);
            analyze.SetData(_dataMock.Object);
            analyze.SetQueryProvider(_queryProviderMock.Object);
            analyze.SetNgram(NgramType.Bigram);
            var result = analyze.AnalyzeString("za przyjeciem");

            Assert.Equal(new List<string> { "za"," ", "przyjêciem"}, result);
        }
    }
}

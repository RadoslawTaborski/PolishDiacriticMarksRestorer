using Xunit;
using NgramAnalyzer;

namespace NgramAnalyzerTests.Unit
{
    public class AnalyzerTests
    {
        [Fact]
        public void AnalyzeStringsReturnsTheSameStrings()
        {
            var analyze= new Analyzer();
            var result = analyze.AnalyzeStrings(new[] {"Kot", "m³ot"});
            Assert.Equal("Kot", result[0]);
            Assert.Equal("m³ot", result[1]);
        }
    }
}

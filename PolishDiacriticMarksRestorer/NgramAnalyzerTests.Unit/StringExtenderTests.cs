using NgramAnalyzer.Common;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class StringExtenderTests
    {
        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 4)]
        [InlineData(3, -1)]
        public void NthIndexOfTests(int n, int result)
        {
            const string str = "small cat";
            var res = str.NthIndexOf("l", n);

            Assert.Equal(result, res);
        }
    }
}

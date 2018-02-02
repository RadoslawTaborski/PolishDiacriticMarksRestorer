using NgramAnalyzer.Common;
using NgramFilter.FilterItems;
using Xunit;

namespace NgramFilterTests.Unit
{
    public class MultipleInstancesTests
    {
        [Fact]
        public void IsCorrect_HasMultipleInstances_False()
        {
            var item = new MultipleInstances();
            var ngram = new NGram
            {
                Value = 1
            };

            var result = item.IsCorrect(ngram);
            Assert.False(result);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(10000000)]
        public void IsCorrect_HasMultipleInstances_True(int value)
        {
            var item = new MultipleInstances();
            var ngram = new NGram
            {
                Value = value
            };

            var result = item.IsCorrect(ngram);
            Assert.True(result);
        }
    }
}

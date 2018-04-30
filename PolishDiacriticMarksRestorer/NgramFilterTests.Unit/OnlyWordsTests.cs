using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.FilterItems;
using Xunit;

namespace NgramFilterTests.Unit
{
    public class OnlyWordsTests
    {
        [Theory]
        [InlineData("small")]
        [InlineData("small,")]
        public void IsCorrect_HasOnlyWords_True(string str)
        {
            var item = new OnlyWords();
            var ngram = new NGram(0, new List<string> { str, "cat" });

            var result = item.IsCorrect(ngram);
            Assert.True(result);
        }

        [Theory]
        [InlineData("-")]
        [InlineData("-,?")]
        public void IsCorrect_HasOnlyWords_False(string str)
        {
            var item = new OnlyWords();
            var ngram = new NGram(0, new List<string> { str, "cat" });

            var result = item.IsCorrect(ngram);
            Assert.False(result);
        }
    }
}

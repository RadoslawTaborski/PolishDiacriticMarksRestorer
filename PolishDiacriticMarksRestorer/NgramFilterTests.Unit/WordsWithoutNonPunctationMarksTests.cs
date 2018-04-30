using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.FilterItems;
using Xunit;

namespace NgramFilterTests.Unit
{
    public class WordsWithoutNonPunctationMarksTests
    {
        [Theory]
        [InlineData("*small")]
        [InlineData("sma#ll")]
        [InlineData("small$")]
        [InlineData("sma,ll")]
        [InlineData("small..")]
        [InlineData("sm,all...")]
        [InlineData("sm.all..")]
        [InlineData("sma$ll.")]
        [InlineData("small....")]
        public void IsCorrect_HasNonPunctionMarks_False(string str)
        {
            var item = new WordsWithoutNonPunctationMarks();
            var ngram = new NGram(0, new List<string> { str, "cat" });

            var result = item.IsCorrect(ngram);
            Assert.False(result);
        }

        [Theory]
        [InlineData("small,")]
        [InlineData("small.")]
        [InlineData("small...")]
        [InlineData("(small")]
        [InlineData(@"""small")]
        [InlineData(@"small)")]
        [InlineData(@"small-cat")]
        public void IsCorrect_HasPunctionMarks_True(string str)
        {
            var item = new WordsWithoutNonPunctationMarks();
            var ngram = new NGram(0, new List<string> { str, "cat" });

            var result = item.IsCorrect(ngram);
            Assert.True(result);
        }
    }
}

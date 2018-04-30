using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.FilterItems;
using Xunit;

namespace NgramFilterTests.Unit
{
    public class NotLongWordsTests
    {
        [Theory]
        [InlineData("smallllllllllllllllllllllllll")]
        public void IsCorrect_TooLongWords_False(string str)
        {
            var item = new NotLongWords();
            var ngram = new NGram(0 , new List<string>{str,"cat"});

            var result = item.IsCorrect(ngram);
            Assert.False(result);
        }

        [Theory]
        [InlineData("small")]
        public void IsCorrect_GoodWords_True(string str)
        {
            var item = new NotLongWords();
            var ngram = new NGram(0, new List<string> {str, "cat"});

            var result = item.IsCorrect(ngram);
            Assert.True(result);
        }
    }
}

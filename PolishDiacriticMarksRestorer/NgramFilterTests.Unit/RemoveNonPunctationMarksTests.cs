using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.ModifierItems;
using Xunit;

namespace NgramFilterTests.Unit
{
    public class RemoveNonPunctationMarksTests
    {
        [Theory]
        [InlineData("small")]
        public void Edit_ReturnTheSameNgram_Equal(string str)
        {
            var item = new RemoveNonPunctationMarks();
            var ngram = new NGram(0, new List<string> { str, "cat" });

            var result = item.Edit(ngram);
            Assert.Equal(ngram,result);
        }
    }
}

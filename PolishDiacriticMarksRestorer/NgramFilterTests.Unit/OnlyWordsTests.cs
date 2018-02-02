using System.Collections.Generic;
using NgramFilter.FilterItems;
using Xunit;

namespace NgramFilterTests.Unit
{
    public class OnlyWordsTests
    {
        [Fact]
        public void Check_HasOnlyWords_True()
        {
            var item = new OnlyWords();
            var list = new List<string>
            {
                "small",
                "cat"
            };

            var result = item.IsCorrect(list);
            Assert.True(result);
        }

        [Fact]
        public void Check_HasOnlyWords_False()
        {
            var item = new OnlyWords();
            var list = new List<string>
            {
                "-",
                "cat"
            };

            var result = item.IsCorrect(list);
            Assert.False(result);
        }
    }
}

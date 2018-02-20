using System.Collections.Generic;
using NgramAnalyzer.Common;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class DictionaryTests
    {
        [Fact]
        public void ChekWordsTests()
        {
            var dict = new List<string>
            {
                "small",
                "cat",
                "dog",
                "mouse",
                "fox"
            };
            var list = new List<string>
            {
                "smaller",
                "cat,",
                "dogs",
                "mouse",
                "foxie"
            };
            var res = new List<string>
            {
                "cat,",
                "mouse",
            };

            var dictionary=new Dictionary(dict);
            var result=dictionary.CheckWords(list);

            Assert.Equal(res,result);
        }

        [Fact]
        public void ChekWordTests()
        {
            var dict = new List<string>
            {
                "small",
                "cat",
                "dog",
                "mouse",
                "fox"
            };

            var dictionary = new Dictionary(dict);
            var result1 = dictionary.CheckWord("cat.");
            var result2 = dictionary.CheckWord("cats;");

            Assert.True(result1);
            Assert.False(result2);
        }
    }
}

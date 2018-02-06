using System.Collections.Generic;
using NgramAnalyzer.Common;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class NGramTests
    {
        [Fact]
        public void ChangeSpecialCharacterToDataBaseStringFormat1()
        {
            var ngram = new NGram
            {
                Value = 15,
                WordsList = new List<string> { @"o\'fehn" }
            };

            ngram.ChangeSpecialCharacters();

            Assert.Equal(@"o\\\'fehn",ngram.WordsList[0]);
        }

        [Fact]
        public void ChangeSpecialCharacterToDataBaseStringFormat2()
        {
            var ngram = new NGram
            {
                Value = 15,
                WordsList = new List<string> { @"milka's" }
            };

            ngram.ChangeSpecialCharacters();

            Assert.Equal(@"milka\'s", ngram.WordsList[0]);
        }

        [Fact]
        public void ToStringTest_SpecialExample()
        {
            var ngram = new NGram
            {
                Value = 15,
                WordsList = new List<string> { "{small}", "cat"}
            };

            var result = ngram.ToString();

            Assert.Equal("15 {{small}} cat", result);
        }

        [Fact]
        public void ToStringTest_NormalExample()
        {
            var ngram = new NGram
            {
                Value = 15,
                WordsList = new List<string> { "small", "cat" }
            };

            var result = ngram.ToString();

            Assert.Equal("15 small cat", result);
        }
    }
}

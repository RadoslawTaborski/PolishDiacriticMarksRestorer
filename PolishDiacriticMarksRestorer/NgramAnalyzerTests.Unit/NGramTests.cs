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

        [Fact]
        public void ToStringsTest()
        {
            var ngram = new NGram
            {
                Value = 15,
                WordsList = new List<string> { "small", "cat" }
            };

            var result = ngram.ToStrings();

            Assert.Equal(new List<string>{"small","cat"}, result);
        }

        [Fact]
        public void Equals_OneItemNull()
        {
            var ngram = new NGram
            {
                Value = 15,
                WordsList = new List<string> { "small", "cat" }
            };

            var ngram2 = new NGram{Value = 15};

            var result = ngram.Equals(ngram2);
            var result2 = ngram2.Equals(ngram);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        public void Equals_DifferentSizeOfWordsList()
        {
            var ngram = new NGram
            {
                Value = 15,
                WordsList = new List<string> { "small", "cat" }
            };

            var ngram2 = new NGram{Value = 15, WordsList = new List<string>()};

            var result = ngram.Equals(ngram2);
            var result2 = ngram2.Equals(ngram);

            Assert.False(result);
            Assert.False(result2);
        }
    }
}

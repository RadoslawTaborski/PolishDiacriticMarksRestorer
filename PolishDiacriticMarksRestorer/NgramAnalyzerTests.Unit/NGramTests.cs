using System.Collections.Generic;
using NgramAnalyzer.Common;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class NGramTests
    {
        [Fact]
        public void ChangeSpecalCharacterToDataBaseStringFormat1()
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
        public void ChangeSpecalCharacterToDataBaseStringFormat2()
        {
            var ngram = new NGram
            {
                Value = 15,
                WordsList = new List<string> { @"milka's" }
            };

            ngram.ChangeSpecialCharacters();

            Assert.Equal(@"milka\'s", ngram.WordsList[0]);
        }
    }
}

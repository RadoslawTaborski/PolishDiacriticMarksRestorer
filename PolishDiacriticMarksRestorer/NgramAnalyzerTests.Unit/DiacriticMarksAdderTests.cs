using System.Collections.Generic;
using NgramAnalyzer;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class DiacriticMarksAdderTests
    {
        private const string Word = "galaz";
        private readonly List<KeyValuePair<string, int>> _res = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("galaz", 0),
            new KeyValuePair<string, int>("galaź", 1),
            new KeyValuePair<string, int>("galaż", 1),
            new KeyValuePair<string, int>("galąz", 1),
            new KeyValuePair<string, int>("galąź", 2),
            new KeyValuePair<string, int>("galąż", 2),
            new KeyValuePair<string, int>("gałaz", 1),
            new KeyValuePair<string, int>("gałaź", 2),
            new KeyValuePair<string, int>("gałaż", 2),
            new KeyValuePair<string, int>("gałąz", 2),
            new KeyValuePair<string, int>("gałąź", 3),
            new KeyValuePair<string, int>("gałąż", 3),
            new KeyValuePair<string, int>("gąlaz", 1),
            new KeyValuePair<string, int>("gąlaź", 2),
            new KeyValuePair<string, int>("gąlaż", 2),
            new KeyValuePair<string, int>("gąląz", 2),
            new KeyValuePair<string, int>("gąląź", 3),
            new KeyValuePair<string, int>("gąląż", 3),
            new KeyValuePair<string, int>("gąłaz", 2),
            new KeyValuePair<string, int>("gąłaź", 3),
            new KeyValuePair<string, int>("gąłaż", 3),
            new KeyValuePair<string, int>("gąłąz", 3),
            new KeyValuePair<string, int>("gąłąź", 4),
            new KeyValuePair<string, int>("gąłąź", 4)
        };

        [Theory]
        [InlineData(5, 24)]
        [InlineData(4, 24)]
        [InlineData(2, 15)]
        public void Start_goodSize(int howManyChanges, int size)
        {
            var adder = new DiacriticMarksAdder();
            var result = adder.Start(Word, howManyChanges);

            Assert.Equal(size, result.Count);
        }

        [Theory]
        [InlineData(4)]
        public void Start_ExistItem(int howManyChanges)
        {
            var adder = new DiacriticMarksAdder();
            var result = adder.Start(Word, howManyChanges);

            foreach (var item in _res)
            {
                var contains = result.Contains(item);
                Assert.True(contains);
            }
        }
    }
}

using Moq;
using NgramAnalyzer.Common;
using Xunit;
using NgramFilter;
using NgramFilter.Interfaces;

namespace NgramFilterTests.Unit
{
    public class FilterTests
    {
        [Fact]
        public void Add_AddsFilterItemToList_Added()
        {
            var itemMock = new Mock<IFilterItem>();
            var filter = new Filter();

            filter.Add(itemMock.Object);

            Assert.Equal(1, filter.Size());
        }

        [Fact]
        public void Size_SizeOfFilterItemList_Verified()
        {
            var itemMock1 = new Mock<IFilterItem>();
            var itemMock2 = new Mock<IFilterItem>();
            var filter = new Filter();

            filter.Add(itemMock1.Object);
            filter.Add(itemMock2.Object);

            Assert.Equal(2, filter.Size());
        }

        [Fact]
        public void Start_NGramIsCorrect_True()
        {
            var mock = new Mock<IFilterItem>();
            var ngram = new NGram();
            mock.Setup(foo => foo.IsCorrect(ngram)).Returns(true);
            var filter = new Filter();

            filter.Add(mock.Object);

            var result = filter.Start(ngram);

            Assert.True(result);
        }

        [Fact]
        public void Start_NGramIsCorrect_False()
        {
            var mock = new Mock<IFilterItem>();
            var ngram = new NGram();
            mock.Setup(foo => foo.IsCorrect(ngram)).Returns(false);
            var filter = new Filter();
            filter.Add(mock.Object);

            var result = filter.Start(ngram);

            Assert.False(result);
        }
    }
}

using System.Collections.Generic;
using Moq;
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
        public void Size_SizeOfFilerItemList_Verified()
        {
            var itemMock1 = new Mock<IFilterItem>();
            var itemMock2 = new Mock<IFilterItem>();
            var filter = new Filter();

            filter.Add(itemMock1.Object);
            filter.Add(itemMock2.Object);

            Assert.Equal(2,filter.Size());
        }

        [Fact]
        public void Start_NGramIsCorrect_True()
        {
            var list = new List<string>
            {
                "small",
                "cat"
            };
            var filter = new Filter();

            var result = filter.Start(list);

            Assert.True(result);
        }

        [Fact]
        public void Start_NGramIsCorrect_False()
        {
            var list = new List<string>
            {
                "-",
                "cat"
            };
            var filter = new Filter();

            var result = filter.Start(list);

            Assert.True(result);
        }
    }
}

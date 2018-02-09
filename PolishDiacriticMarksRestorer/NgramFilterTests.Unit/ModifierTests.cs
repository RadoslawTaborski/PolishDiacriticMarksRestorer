using Moq;
using NgramAnalyzer.Common;
using Xunit;
using NgramFilter;
using NgramFilter.Interfaces;

namespace NgramFilterTests.Unit
{
    public class ModifierTests
    {
        [Fact]
        public void Add_AddsModifierItemToList_Added()
        {
            var itemMock = new Mock<IModifierItem>();
            var modifier = new Modifier();

            modifier.Add(itemMock.Object);

            Assert.Equal(1, modifier.Size());
        }

        [Fact]
        public void Size_SizeOfModifierItemList_Verified()
        {
            var itemMock1 = new Mock<IModifierItem>();
            var itemMock2 = new Mock<IModifierItem>();
            var modifier = new Modifier();

            modifier.Add(itemMock1.Object);
            modifier.Add(itemMock2.Object);

            Assert.Equal(2, modifier.Size());
        }

        [Fact]
        public void Start_NGramEdited()
        {
            var mock = new Mock<IModifierItem>();
            var ngram1 = new NGram {Value = 10};
            var ngram2 = new NGram{Value = 15};
            mock.Setup(foo => foo.Edit(ngram1)).Returns(ngram2);
            var modifier = new Modifier();

            modifier.Add(mock.Object);

            var result = modifier.Start(ngram1);

            Assert.NotEqual(ngram1,result);
        }

        [Fact]
        public void Start_NGramUnedit()
        {
            var mock = new Mock<IModifierItem>();
            var ngram1 = new NGram { Value = 10 };
            mock.Setup(foo => foo.Edit(ngram1)).Returns(ngram1);
            var modifier = new Modifier();

            modifier.Add(mock.Object);

            var result = modifier.Start(ngram1);

            Assert.Equal(ngram1, result);
        }
    }
}

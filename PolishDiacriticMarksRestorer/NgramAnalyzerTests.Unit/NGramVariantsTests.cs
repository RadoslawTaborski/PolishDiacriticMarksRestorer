using System.Collections.Generic;
using Moq;
using NgramAnalyzer;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class NGramVariantsTests
    {
        [Fact]
        public void SetVariants()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();
            marksAdderMock.Setup(m => m.Start("jest", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest",0),
                new KeyValuePair<string, int>("jęst",1),
                new KeyValuePair<string, int>("jeśt",1),
                new KeyValuePair<string, int>("jęśt",2),
            });

            marksAdderMock.Setup(m => m.Start("ktora", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("ktora",0),
                new KeyValuePair<string, int>("która",1),
                new KeyValuePair<string, int>("którą",2),
            });

            var variants = new NGramVariants(new NGram(5, new List<string> { "jest", "ktora" }), marksAdderMock.Object);
            variants.CreateVariants(new List<string> { "jest", "która", "którą" });

            var variantsResult = new List<NGram>
            {
                new NGram(0, new List<string>{"jest", "która"}),
                new NGram(0, new List<string>{"jest", "którą"}),
            };

            foreach (var item in variantsResult)
            {
                var content = variants.NgramVariants.Exists(x=>x.Ngram.Equals(item));
                Assert.True(content);
            }
            Assert.Equal(2, variants.NgramVariants.Count);
        }

        [Fact]
        public void UpdateNGramsVariantes()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();
            marksAdderMock.Setup(m => m.Start("jest", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest",0),
                new KeyValuePair<string, int>("jęst",1),
                new KeyValuePair<string, int>("jeśt",1),
                new KeyValuePair<string, int>("jęśt",2),
            });

            marksAdderMock.Setup(m => m.Start("ktora", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("ktora",0),
                new KeyValuePair<string, int>("która",1),
                new KeyValuePair<string, int>("którą",2),
            });

            var list = new List<NGram>
            {
                new NGram(15, new List<string>{"jest","która"})
            };

            var variantsResult = new List<NGram>
            {
                new NGram( 15,  new List<string>{"jest", "która"}),
                new NGram( 0,  new List<string>{"jest", "którą"}),
            };

            var variants = new NGramVariants(new NGram(5, new List<string> { "jest", "ktora" }), marksAdderMock.Object);
            variants.CreateVariants(new List<string> { "jest", "która", "którą" });
            variants.UpdateNGramsVariants(list);

            foreach (var item in variantsResult)
            {
                var content = variants.NgramVariants.Exists(x => x.Ngram.Equals(item));
                Assert.True(content);
            }

            Assert.Equal(2, variants.NgramVariants.Count);
        }

        [Fact]
        public void VariantsToStringsLists_Normal()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();
            marksAdderMock.Setup(m => m.Start("jest", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest",0),
                new KeyValuePair<string, int>("jęst",1),
                new KeyValuePair<string, int>("jeśt",1),
                new KeyValuePair<string, int>("jęśt",2),
            });

            marksAdderMock.Setup(m => m.Start("ktora", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("ktora",0),
                new KeyValuePair<string, int>("która",1),
                new KeyValuePair<string, int>("którą",2),
            });

            var variantsResult = new List<List<string>>
            {
                new List<string>{"jest"},
                new List<string>{"która", "którą"}
            };

            var variants = new NGramVariants(new NGram(5, new List<string> { "jest", "ktora" }), marksAdderMock.Object);
            variants.CreateVariants(new List<string> { "jest", "która", "którą" });
            var result = variants.VariantsToStringsLists();

            Assert.Equal(variantsResult, result);
        }

        [Fact]
        public void VariantsToStringsLists_VariantsCount0()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();

            var variants = new NGramVariants(new NGram(5, new List<string> { "jest", "ok" }), marksAdderMock.Object);
            var result = variants.VariantsToStringsLists();

            Assert.Null(result);
        }

        [Fact]
        public void RestoreUpperLettersInVariants_Normal()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();
            marksAdderMock.Setup(m => m.Start("jest", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest",0),
                new KeyValuePair<string, int>("jęst",1),
                new KeyValuePair<string, int>("jeśt",1),
                new KeyValuePair<string, int>("jęśt",2),
            });

            marksAdderMock.Setup(m => m.Start("która", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("która",0),
                new KeyValuePair<string, int>("którą",1),
            });

            var variantsResult = new List<NGram>
            {
                new NGram(0, new List<string>{"JeSt", "KtÓRa"}),
                new NGram(0, new List<string>{"JeSt", "KtÓRą"}),
            };

            var variants = new NGramVariants(new NGram(5, new List<string> { "JeSt", "KtÓRa" }), marksAdderMock.Object);
            variants.CreateVariants(new List<string> { "jest", "która", "którą" });
            variants.RestoreUpperLettersInVariants();

            foreach (var item in variantsResult)
            {
                var content = variants.NgramVariants.Exists(x => x.Ngram.Equals(item));
                Assert.True(content);
            }
            Assert.Equal(2, variants.NgramVariants.Count);
        }
    }
}

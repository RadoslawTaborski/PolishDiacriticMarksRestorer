using System;
using System.Collections.Generic;
using System.Text;
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

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ktora" } }, marksAdderMock.Object);
            variants.CreateVariants(new List<string>{"jest", "która", "którą"});

            var variantsResult = new List<NGram>
            {
                new NGram{ WordsList = new List<string>{"jest", "która"}},
                new NGram{ WordsList = new List<string>{"jest", "którą"}},
            };

            foreach (var item in variantsResult)
            {
                var content = variants.NgramVariants.Contains(item);
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
                new NGram{Value = 15, WordsList = new List<string>{"jest","która"}}
            };

            var variantsResult = new List<NGram>
            {
                new NGram{Value = 15, WordsList = new List<string>{"jest", "która"}},
                new NGram{ WordsList = new List<string>{"jest", "którą"}},
            };

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ktora" } }, marksAdderMock.Object);
            variants.CreateVariants(new List<string> { "jest", "która", "którą" });
            variants.UpdateNGramsVariants(list);

            foreach (var item in variantsResult)
            {
                var content = variants.NgramVariants.Contains(item);
                Assert.True(content);
            }

            Assert.Equal(2,variants.NgramVariants.Count);
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

            var list = new List<NGram>
            {
                new NGram{Value = 15, WordsList = new List<string>{"jest","ok"}}
            };

            var variantsResult = new List<List<string>>
            {
                new List<string>{"jest"},
                new List<string>{"która", "którą"}
            };

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ktora" } }, marksAdderMock.Object);
            variants.CreateVariants(new List<string> { "jest", "która", "którą" });
            var result=variants.VariantsToStringsLists();

            Assert.Equal(variantsResult, result);
        }

        [Fact]
        public void VariantsToStringsLists_VariantsCount0()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ok" } }, marksAdderMock.Object);
            var result = variants.VariantsToStringsLists();

            Assert.Null(result);
        }
    }
}

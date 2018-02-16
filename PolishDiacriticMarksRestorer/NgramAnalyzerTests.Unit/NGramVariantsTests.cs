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
            marksAdderMock.Setup(m => m.Start("jest ok", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest ok",0),
                new KeyValuePair<string, int>("jęst ok",1),
                new KeyValuePair<string, int>("jeśt ok",1),
                new KeyValuePair<string, int>("jest ók",1),
                new KeyValuePair<string, int>("jęśt ok",2),
                new KeyValuePair<string, int>("jęst ók",2),
                new KeyValuePair<string, int>("jeśt ók",2),
                new KeyValuePair<string, int>("jęśt ók",3),
            });

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ok" } }, marksAdderMock.Object);
            variants.CreateVariants();

            var variantsResult = new List<NGram>
            {
                new NGram{ WordsList = new List<string>{"jest", "ok"}},

                new NGram{ WordsList = new List<string>{"jęst", "ok"}},
                new NGram{ WordsList = new List<string>{"jeśt", "ok"}},
                new NGram{ WordsList = new List<string>{"jest", "ók"}},

                new NGram{ WordsList = new List<string>{"jęśt", "ok"}},
                new NGram{ WordsList = new List<string>{"jęst", "ók"}},
                new NGram{ WordsList = new List<string>{"jeśt", "ók"}},

                new NGram{ WordsList = new List<string>{"jęśt", "ók"}},
            };

            foreach (var item in variantsResult)
            {
                var content = variants.NgramVariants.Contains(item);
                Assert.True(content);
            }
            Assert.Equal(8, variants.NgramVariants.Count);
        }

        [Fact]
        public void VariantsToWordList()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();
            marksAdderMock.Setup(m => m.Start("jest ok", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest ok",0),
                new KeyValuePair<string, int>("jęst ok",1),
                new KeyValuePair<string, int>("jeśt ok",1),
                new KeyValuePair<string, int>("jest ók",1),
                new KeyValuePair<string, int>("jęśt ok",2),
                new KeyValuePair<string, int>("jęst ók",2),
                new KeyValuePair<string, int>("jeśt ók",2),
                new KeyValuePair<string, int>("jęśt ók",3),
            });
            var res = new List<string>
            {
                "jest", "ok", "jęst", "jeśt", "ók", "jęśt"
            };

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ok" } }, marksAdderMock.Object);
            variants.CreateVariants();
            var result = variants.VariantsToWordList();

            Assert.Equal(res,result);
        }

        [Fact]
        public void LeaveGoodVariants()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();
            marksAdderMock.Setup(m => m.Start("jest ok", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest ok",0),
                new KeyValuePair<string, int>("jęst ok",1),
                new KeyValuePair<string, int>("jeśt ok",1),
                new KeyValuePair<string, int>("jest ók",1),
                new KeyValuePair<string, int>("jęśt ok",2),
                new KeyValuePair<string, int>("jęst ók",2),
                new KeyValuePair<string, int>("jeśt ók",2),
                new KeyValuePair<string, int>("jęśt ók",3),
            });
            var list = new List<string>
            {
                "jest", "ok"
            };

            var variantsResult = new List<NGram>
            {
                new NGram{ WordsList = new List<string>{"jest", "ok"}},
            };

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ok" } }, marksAdderMock.Object);
            variants.CreateVariants();
            variants.LeaveGoodVariants(list);

            foreach (var item in variantsResult)
            {
                var content = variants.NgramVariants.Contains(item);
                Assert.True(content);
            }

            Assert.Single(variants.NgramVariants);
        }

        [Fact]
        public void UpdateNGramsVariantes()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();
            marksAdderMock.Setup(m => m.Start("jest ok", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest ok",0),
                new KeyValuePair<string, int>("jęst ok",1),
                new KeyValuePair<string, int>("jeśt ok",1),
            });
            var list = new List<NGram>
            {
                new NGram{Value = 15, WordsList = new List<string>{"jest","ok"}}
            };

            var variantsResult = new List<NGram>
            {
                new NGram{Value = 15, WordsList = new List<string>{"jest", "ok"}},
                new NGram{ WordsList = new List<string>{"jęst", "ok"}},
                new NGram{ WordsList = new List<string>{"jeśt", "ok"}},
            };

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ok" } }, marksAdderMock.Object);
            variants.CreateVariants();
            variants.UpdateNGramsVariants(list);

            foreach (var item in variantsResult)
            {
                var content = variants.NgramVariants.Contains(item);
                Assert.True(content);
            }

            Assert.Equal(3,variants.NgramVariants.Count);
        }

        [Fact]
        public void VariantsToStringsLists_Normal()
        {
            var marksAdderMock = new Mock<IDiacriticMarksAdder>();
            marksAdderMock.Setup(m => m.Start("jest ok", It.IsAny<int>())).Returns(new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("jest ok",0),
                new KeyValuePair<string, int>("jęst ok",1),
                new KeyValuePair<string, int>("jeśt ok",1),
            });
            var list = new List<NGram>
            {
                new NGram{Value = 15, WordsList = new List<string>{"jest","ok"}}
            };

            var variantsResult = new List<List<string>>
            {
                new List<string>{"jest", "jęst", "jeśt"},
                new List<string>{"ok"}
            };

            var variants = new NGramVariants(new NGram { Value = 5, WordsList = new List<string> { "jest", "ok" } }, marksAdderMock.Object);
            variants.CreateVariants();
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

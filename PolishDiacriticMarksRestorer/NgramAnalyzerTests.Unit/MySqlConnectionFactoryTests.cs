using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using NgramAnalyzer.Common;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class MySqlConnectionFactoryTests
    {
        [Fact]
        public void CreateConnectionDb_NotNull()
        {
            var sut = new MySqlConnectionFactory();
            var result = sut.CreateConnectionDb("");

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateConnectionServer_NotNull()
        {
            var sut = new MySqlConnectionFactory();
            var result = sut.CreateConnectionServer("");

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateDataAdapter_NotNull()
        {
            var sut = new MySqlConnectionFactory();
            var result = sut.CreateDataAdapter("");

            Assert.NotNull(result);
        }
    }
}

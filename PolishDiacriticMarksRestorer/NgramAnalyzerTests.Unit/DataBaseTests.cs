using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class DataBaseTests
    {
        [Fact]
        public void ConnectAndDisconnectFromDatabase()
        {
            IDataAccess db = new DataBase("localhost", "testowa", "root", "");

            var connected = db.Connect();
            var disconnected = db.Disconnect();

            Assert.True(connected);
            Assert.True(disconnected);
        }

        [Fact]
        public void ExecuteSqlCommand()
        {
            const string query = "SELECT * FROM `dane`";
            IDataAccess db = new DataBase("localhost", "testowa", "root", "");
            var dt = db.ExecuteSqlCommand(query);
            Assert.NotNull(dt);
        }
    }
}

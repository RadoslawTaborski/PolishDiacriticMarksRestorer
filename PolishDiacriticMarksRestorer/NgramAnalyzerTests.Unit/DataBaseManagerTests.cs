using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using Xunit;

namespace NgramAnalyzerTests.Unit
{
    public class DataBaseManagerTests
    {
        [Fact]
        public void ConnectAndDisconnectFromDatabase()
        {
            IDataAccess db = new DataBaseManager("localhost", "testowa", "root", "");

            var connected = db.ConnectToDb();
            var disconnected = db.Disconnect();

            Assert.True(connected);
            Assert.True(disconnected);
        }

        [Fact]
        public void ExecuteSqlCommand()
        {
            const string query = "SELECT * FROM `dane`";
            IDataAccess db = new DataBaseManager("localhost", "testowa", "root", "");
            var dt = db.ExecuteSqlCommand(query);
            Assert.NotNull(dt);
        }
    }
}

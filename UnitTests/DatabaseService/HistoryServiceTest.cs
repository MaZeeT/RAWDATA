using DatabaseService.Services;
using Xunit;
using Xunit.Sdk;

namespace UnitTests.DatabaseService
{
    public class HistoryServiceTest
    {
        
        [Fact]
        public void HistoryAddValid()
        {
            var service = new HistoryService();

            Assert.True(false);
        }    
        
        [Fact]
        public void HistoryAddInvalid()
        {
            var service = new HistoryService();

            Assert.True(false);
        }

        [Fact]
        public void HistoryDeleteValid()
        {
            var service = new HistoryService();

            Assert.True(false);
        }
        
        [Fact]
        public void HistoryDeleteInvalid()
        {
            var service = new HistoryService();

            Assert.True(false);
        }

        [Fact]
        public void HistoryExistTrue()
        {
            var service = new HistoryService();
            var historyId = 0; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.HistoryExist(historyId));
        }
        
        [Fact]
        public void HistoryExistFalse()
        {
            var service = new HistoryService();
            var historyId = -1; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.HistoryExist(historyId));
        }
        
        [Fact]
        public void HistoryGetValid()
        {
            var service = new HistoryService();

            Assert.True(false);
        }
        
        [Fact]
        public void HistoryGetInvalid()
        {
            var service = new HistoryService();

            Assert.True(false);
        }
        
    }
}
using System;
using DatabaseService.Modules;
using DatabaseService.Services;
using Xunit;

namespace UnitTests.DatabaseService
{
    public class HistoryServiceTest
    {
        [Fact]
        public void HistoryAddValid()
        {
            var service = new HistoryService();
            var history = new History
            {
                Id = 1,
                Userid = 1,
                Postid = 1,
                PostTableName = "test",
                Date = new DateTime(2008, 2, 1),
                isBookmark = true
            };

            Assert.True(service.Add(history));
            
            //clean up todo delete when mock is working
            service.Delete(history.Id);
        }

        [Fact]
        public void HistoryAddInvalid()
        {
            var service = new HistoryService();
            var history = new History
            {
                Id = -5,
                Userid = 15,
                Postid = 110,
                PostTableName = "tester",
                Date = new DateTime(2108, 7, 5),
                isBookmark = false
            };

            Assert.False(service.Add(history));
        }

        [Fact]
        public void HistoryDeleteValid()
        {
            var service = new HistoryService();
            var history = new History
            {
                Id = 5,
                Userid = 15,
                Postid = 110,
                PostTableName = "testing",
                Date = new DateTime(1988, 6, 5),
                isBookmark = false
            };

            Assert.True(service.Add(history));
            Assert.True(service.Delete(history.Id));
        }

        [Fact]
        public void HistoryDeleteInvalid()
        {
            var service = new HistoryService();
            var history = new History
            {
                Id = 5,
                Userid = 15,
                Postid = 110,
                PostTableName = "testing",
                Date = new DateTime(1988, 6, 5),
                isBookmark = false
            };

            Assert.True(service.Add(history));
            Assert.True(service.Delete(-5));
            
            //clean up todo delete when mock is working
            service.Delete(history.Id);
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
            var historyId = -8; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.HistoryExist(historyId));
        }

        [Fact]
        public void HistoryGetValid()
        {
            var service = new HistoryService();
            var id = 3;
            var history = new History
            {
                Id = id,
                Userid = 25,
                Postid = 89,
                PostTableName = "this is a nice tester",
                Date = new DateTime(1998, 2, 15),
                isBookmark = true
            };

            var historyGet = service.Get(id);

            Assert.True(service.Add(history));
            Assert.Equal(history.Id, historyGet.Id);
            Assert.Equal(history.Userid, historyGet.Userid);
            Assert.Equal(history.Postid, historyGet.Postid);
            Assert.Equal(history.PostTableName, historyGet.PostTableName);
            Assert.Equal(history.Date, historyGet.Date);
            Assert.Equal(history.isBookmark, historyGet.isBookmark);

            //clean up todo delete when mock is working
            service.Delete(history.Id);
        }

        [Fact]
        public void HistoryGetInvalid()
        {
            var service = new HistoryService();
            var id = -31;

            var history = service.Get(id);

            Assert.Null(history);
        }
        
    }
}
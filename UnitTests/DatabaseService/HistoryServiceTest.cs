using System;
using DatabaseService.Modules;
using DatabaseService.Services;
using Xunit;

namespace UnitTests.DatabaseService
{
    public class HistoryServiceTest
    {
        [Fact]
        public void HistoryAddInvalid()
        {
            var service = new HistoryService();
            var history = new History
            {
                Userid = 15,
                Postid = 110,
                isBookmark = false
            };

            Assert.False(service.Add(history));
        }

        [Fact]
        public void HistoryAddValid()
        {
            var service = new HistoryService();
            var history = new History
            {
                Userid = 0,
                Postid = 1760,
                isBookmark = false
            };

            var result = service.Add(history);
            
            Assert.True(result);

            //clean up todo delete when mock is working
            service.Delete(service.Get(0, 1760).Id);
        }

        [Fact]
        public void HistoryDeleteInvalid()
        {
            var service = new HistoryService();

            var Userid = -5;

            Assert.False(service.Delete(Userid));
        }

        [Fact]
        public void HistoryDeleteValid()
        {
            var service = new HistoryService();
            var Userid = 0;
            var Postid = 709;
            var isBookmark = true;
            var historyToAdd = new History {Userid = Userid, Postid = Postid, isBookmark = isBookmark};

            var resultAdd = service.Add(historyToAdd);
            var history = service.Get(Userid, Postid);

            Assert.True(resultAdd);
            Assert.True(service.HistoryExist(history.Id));
            Assert.True(service.Delete(history.Id));
            Assert.False(service.HistoryExist(history.Id));
        }

        [Fact]
        public void HistoryExistFalse()
        {
            var service = new HistoryService();
            var historyId = -8; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.HistoryExist(historyId));
        }

        [Fact]
        public void HistoryExistTrue()
        {
            var service = new HistoryService();

            var historyId = 4; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.HistoryExist(historyId));
        }

        [Fact]
        public void HistoryGetInvalid()
        {
            var service = new HistoryService();
            var id = -31;

            var history = service.Get(id);

            Assert.Null(history);
        }
        
        [Fact]
        public void HistoryGetInvalid2()
        {
            var service = new HistoryService();
            var Userid = -31;
            var Postid = -123;

            var history = service.Get(Userid, Postid);

            Assert.Null(history);
        }

        [Fact]
        public void HistoryGetValid()
        {
            var service = new HistoryService();

            var Userid = 0;
            var Postid = 709;
            var isBookmark = true;
            var history = new History {Userid = Userid, Postid = Postid, isBookmark = isBookmark};

            var histroyAdd = service.Add(history);
            var historyGet = service.Get(Userid, Postid);

            //todo fix this
            Assert.True(histroyAdd);
            Assert.Equal(Userid, historyGet.Userid);
            Assert.Equal(Postid, historyGet.Postid);
            Assert.Equal(isBookmark, historyGet.isBookmark);

            //clean up todo delete when mock is working
            service.Delete(history.Id);
        }
    }
}
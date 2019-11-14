using System;
using System.Collections.Generic;
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
        public void HistoryDeleteBookmarkInvalidPost()
        {
            var service = new HistoryService();

            var invalidModifier = -1;
            var userid = 290;
            var postId = 1760;

            var resultAdd = service.Add(userid, postId, true);
            
            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid, postId * invalidModifier));

            //clean up todo delete when mock is working
            service.Delete(service.Get(userid, postId).Id);
        }

        [Fact]
        public void HistoryDeleteBookmarkInvalidUser()
        {
            var service = new HistoryService();

            var invalidModifier = -1;
            var userid = 290;
            var postId = 709;

            var resultAdd = service.Add(userid, postId, true);
            
            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid * invalidModifier, postId));

            //clean up todo delete when mock is working
            service.Delete(service.Get(userid, postId).Id);
        }

        [Fact]
        public void HistoryDeleteBookmarkInvalidUserAndPost()
        {
            var service = new HistoryService();

            var invalidModifier = -1;
            var userid = 290;
            var postId = 1711;

            var resultAdd = service.Add(userid, postId, true);
            
            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid * invalidModifier, postId * invalidModifier));

            //clean up todo delete when mock is working
            service.Delete(service.Get(userid, postId).Id);
        }

        [Fact]
        public void HistoryDeleteBookmarkValid()
        {
            var service = new HistoryService();
            
            var userid = 290;
            var postId = 1760;

            var resultAdd = service.Add(userid, postId, true);
            
            Assert.True(resultAdd);
            Assert.True(service.DeleteBookmark(userid, postId));

            //clean up todo delete when mock is working
            service.Delete(service.Get(userid, postId).Id);
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
        public void GetHistoryList()
        {
            var service = new HistoryService();
            var Userid = 290;

            var Postid1 = 19;
            var Postid2 = 709;
            var Postid3 = 1760;

            var addresult1 = service.Add(Userid, Postid1, false);
            var addresult2 = service.Add(Userid, Postid2, true);
            var addresult3 = service.Add(Userid, Postid3, false);
            var history = service.GetHistoryList(Userid);


            //clean up todo delete when mock is working
            service.Delete(service.Get(Userid, Postid1).Id);
            service.Delete(service.Get(Userid, Postid2).Id);
            service.Delete(service.Get(Userid, Postid3).Id);
            //end of clean up            

            Assert.True(addresult1);
            Assert.True(addresult2);
            Assert.True(addresult3);

            Assert.Equal(3, history.Count);
            Assert.Equal(Postid3, history[2].Postid);
            Assert.Equal(Postid2, history[1].Postid);
            Assert.Equal(Postid1, history[0].Postid);
        }

        [Fact]
        public void GetBookmarks()
        {
            var service = new HistoryService();
            var Userid = 290;

            var Postid1 = 19;
            var Postid2 = 709;
            var Postid3 = 1760;
            var Postid4 = 1711;

            var addresult1 = service.Add(Userid, Postid1, false);
            var addresult2 = service.Add(Userid, Postid2, true);
            var addresult3 = service.Add(Userid, Postid3, true);
            var addresult4 = service.Add(Userid, Postid4, false);
            var history = service.GetBookmarkList(Userid);


            //clean up todo delete when mock is working
            service.Delete(service.Get(Userid, Postid1).Id);
            service.Delete(service.Get(Userid, Postid2).Id);
            service.Delete(service.Get(Userid, Postid3).Id);
            service.Delete(service.Get(Userid, Postid4).Id);
            //end of clean up            

            Assert.True(addresult1);
            Assert.True(addresult2);
            Assert.True(addresult3);
            Assert.True(addresult4);

            Assert.Equal(2, history.Count);
            Assert.Equal(Postid3, history[1].Postid);
            Assert.Equal(Postid2, history[0].Postid);
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
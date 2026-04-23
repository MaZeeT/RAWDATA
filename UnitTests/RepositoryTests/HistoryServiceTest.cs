using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess.Database;
using Infrastructure.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UnitTests.RepositoryTests
{
    public class HistoryRepositoryTest
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;
        private readonly ServiceProvider _serviceProvider;
        private const int TestUserId = 40;
        private readonly DateTime _dateTime = new DateTime(2023,11,24,22,12,44);
        
        public HistoryRepositoryTest()
        {
            const string database = "host=localhost;port=5432;db=stackoverflow;uid=postgres;pwd=Password123";
            var services = new ServiceCollection();
            services.AddSingleton<IHistoryRepository, HistoryRepository>();
            services.AddDbContextFactory<DatabaseContext>(options =>
            {
                options
                    .UseLoggerFactory(DatabaseContext.MyLoggerFactory)
                    .UseNpgsql(database);
            });

            _serviceProvider = services.BuildServiceProvider();
            _dbContextFactory =  _serviceProvider.GetRequiredService<IDbContextFactory<DatabaseContext>>();
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum" 
                      + "checking for valid user is moved to service layer")] //Todo move test to service layer
        public void HistoryAddInvalid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            var history = new History
            {
                UserId = TestUserId,
                PostId = 110,
                IsBookmark = false
            };

            Assert.False(service.Add(history));
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryAddValid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            var history = new History
            {
                UserId = TestUserId,
                PostId = 1760,
                IsBookmark = false
            };

            var result = service.Add(history);

            Assert.True(result);

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Fetch(TestUserId, 1760).Id);
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryDeleteBookmarkInvalidPost()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int invalidModifier = -1;
            const int userid = TestUserId;
            const int postId = 1760;

            var history = new History
            {
                UserId = userid,
                PostId = postId,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = true
            };

            var resultAdd = service.Add(history);

            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid, postId * invalidModifier));

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Fetch(userid, postId).Id);
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryDeleteBookmarkInvalidUser()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int invalidModifier = -1;
            const int userid = TestUserId;
            const int postId = 709;
            
            var history = new History
            {
                UserId = userid,
                PostId = postId,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = true
            };

            var resultAdd = service.Add(history);

            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid * invalidModifier, postId));

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Fetch(userid, postId).Id);
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryDeleteBookmarkInvalidUserAndPost()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int invalidModifier = -1;
            const int userid = TestUserId;
            const int postId = 1711;

            var history = new History
            {
                UserId = userid,
                PostId = postId,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = true
            };

            var resultAdd = service.Add(history);

            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid * invalidModifier, postId * invalidModifier));

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Fetch(userid, postId).Id);
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryDeleteBookmarkValid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int userid = TestUserId;
            const int postId = 1760;

            var history = new History
            {
                UserId = userid,
                PostId = postId,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = true
            };

            var resultAdd = service.Add(history);

            Assert.True(resultAdd);
            
            var resultDelete = service.DeleteBookmark(userid, postId);
            Assert.True(resultDelete);

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Fetch(userid, postId).Id);
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryDeleteUserEmptyHistory()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userid = 12;

            var historyPre = service.GetHistoryList(userid);
            var historyDeletion = service.DeleteUserHistory(userid);
            var historyPost = service.GetHistoryList(userid);

            Assert.Empty(historyPre);
            Assert.False(historyDeletion);
            Assert.Empty(historyPost);
        }


        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryDeleteUserHistory()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userid = TestUserId;

            const int postId1 = 19;
            const int postId2 = 709;
            const int postId3 = 1760;
            const int postId4 = 1711;
            
            var history1 = new History
            {
                UserId = userid,
                PostId = postId1,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = false
            };
            
            var history2 = new History
            {
                UserId = userid,
                PostId = postId2,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = false
            };
            
            var history3 = new History
            {
                UserId = userid,
                PostId = postId3,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = true
            };
            
            var history4 = new History
            {
                UserId = userid,
                PostId = postId4,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = false
            };

            var addResult1 = service.Add(history1);
            var addResult2 = service.Add(history2);
            var addResult3 = service.Add(history3);
            var addResult4 = service.Add(history4);

            var historyPre = service.GetHistoryList(userid);
            var historyDeletion = service.DeleteUserHistory(userid);
            var historyPost = service.GetHistoryList(userid);

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Fetch(userid, postId3).Id);
            //end of clean up

            Assert.True(addResult1);
            Assert.True(addResult2);
            Assert.True(addResult3);
            Assert.True(addResult4);

            Assert.Equal(3, historyPre.Count);
            Assert.True(historyDeletion);
            Assert.Empty(historyPost);
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryDeleteInvalid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int userId = -5;

            Assert.False(service.DeleteHistory(userId));
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryDeleteValid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userId = TestUserId;
            const int postId = 709;
            const bool isBookmark = true;
            var historyToAdd = new History {UserId = userId, PostId = postId, IsBookmark = isBookmark};

            var resultAdd = service.Add(historyToAdd);
            var history = service.Fetch(userId, postId);

            Assert.True(resultAdd);
            Assert.True(service.HistoryExist(history.Id));
            Assert.True(service.DeleteHistory(history.Id));
            Assert.False(service.HistoryExist(history.Id));
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryExistFalse()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int historyId = -8; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.HistoryExist(historyId));
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryExistTrue()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int historyId = 11; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.HistoryExist(historyId));
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryGetInvalid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int id = -31;

            Assert.Throws<ArgumentException>(() => service.Fetch(id));
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryGetInvalid2()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userId = -31;
            const int postId = -123;

            Assert.Throws<ArgumentException>(() => service.Fetch(userId, postId));
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void GetHistoryList()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userId = TestUserId;

            const int postId1 = 19;
            const int postId2 = 709;
            const int postId3 = 1760;
            
            var history1 = new History
            {
                UserId = userId,
                PostId = postId1,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = false
            };
            
            var history2 = new History
            {
                UserId = userId,
                PostId = postId2,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = true
            };
            
            var history3 = new History
            {
                UserId = userId,
                PostId = postId3,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = false
            };
            

            var addResult1 = service.Add(history1);
            var addResult2 = service.Add(history2);
            var addResult3 = service.Add(history3);
            var history = service.GetHistoryList(userId);


            //clean up todo delete when mock is working
            service.DeleteHistory(service.Fetch(userId, postId1).Id);
            service.DeleteHistory(service.Fetch(userId, postId2).Id);
            service.DeleteHistory(service.Fetch(userId, postId3).Id);
            //end of clean up            

            Assert.True(addResult1);
            Assert.True(addResult2);
            Assert.True(addResult3);

            Assert.Equal(2, history.Count);
            Assert.Equal(postId3, history[1].PostId);
            Assert.Equal(postId1, history[0].PostId);
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void GetBookmarks()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userId = TestUserId;

            const int postId1 = 19;
            const int postId2 = 709;
            const int postId3 = 1760;
            const int postId4 = 1711;

            var history1 = new History
            {
                UserId = userId,
                PostId = postId1,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = false
            };
            
            var history2 = new History
            {
                UserId = userId,
                PostId = postId2,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = true
            };
            
            var history3 = new History
            {
                UserId = userId,
                PostId = postId3,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = true
            };
            
            var history4 = new History
            {
                UserId = userId,
                PostId = postId4,
                PostTableName = "questions",
                Date = _dateTime,
                IsBookmark = false
            };

            var addResult1 = service.Add(history1);
            var addResult2 = service.Add(history2);
            var addResult3 = service.Add(history3);
            var addResult4 = service.Add(history4);
            var history = service.GetBookmarkList(userId);


            //clean up todo delete when mock is working
            service.DeleteHistory(service.Fetch(userId, postId1).Id);
            service.DeleteHistory(service.Fetch(userId, postId2).Id);
            service.DeleteHistory(service.Fetch(userId, postId3).Id);
            service.DeleteHistory(service.Fetch(userId, postId4).Id);
            //end of clean up            

            Assert.True(addResult1);
            Assert.True(addResult2);
            Assert.True(addResult3);
            Assert.True(addResult4);

            Assert.Equal(2, history.Count);
            Assert.Equal(postId3, history[1].PostId);
            Assert.Equal(postId2, history[0].PostId);
        }

        [Fact (Skip = "Rewrite into testing the service layer and not the repositories which should be dum")]
        public void HistoryGetValid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int userId = TestUserId;
            const int postId = 709;
            const bool isBookmark = true;
            var history = new History {UserId = userId, PostId = postId, IsBookmark = isBookmark};

            var historyAdd = service.Add(history);
            var historyGet = service.Fetch(userId, postId);

            //todo fix this
            Assert.True(historyAdd);
            Assert.Equal(userId, historyGet.UserId);
            Assert.Equal(postId, historyGet.PostId);
            Assert.Equal(isBookmark, historyGet.IsBookmark);

            //clean up todo delete when mock is working
            service.DeleteHistory(history.Id);
        }
    }
}
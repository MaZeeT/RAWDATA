using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Implementation;
using Repositories.Interfaces;
using Xunit;

namespace Tests.Infrastructure.IntegrationTests
{
    public class HistoryRepositoryTest
    {
        private readonly IDbContextFactory<DatabaseContext2> _dbContextFactory;
        private readonly ServiceProvider _serviceProvider;
        private const int testUserId = 40;
        
        public HistoryRepositoryTest()
        {
            const string database = "host=localhost;port=5432;db=stackoverflow;uid=postgres;pwd=Password123";
            var services = new ServiceCollection();
            services.AddSingleton<IHistoryRepository, HistoryRepository>();
            services.AddPooledDbContextFactory<DatabaseContext2>(options =>
            {
                options
                    .UseLoggerFactory(DatabaseContext2.MyLoggerFactory)
                    .UseNpgsql(database);
            });

            _serviceProvider = services.BuildServiceProvider();
            _dbContextFactory =  _serviceProvider.GetRequiredService<IDbContextFactory<DatabaseContext2>>();
        }

        [Fact]
        public void HistoryAddInvalid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            var history = new History
            {
                UserId = testUserId,
                PostId = 110,
                IsBookmark = false
            };

            Assert.False(service.Add(history));
        }

        [Fact]
        public void HistoryAddValid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            var history = new History
            {
                UserId = testUserId,
                PostId = 1760,
                IsBookmark = false
            };

            var result = service.Add(history);

            Assert.True(result);

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Get(testUserId, 1760).Id);
        }

        [Fact]
        public void HistoryDeleteBookmarkInvalidPost()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int invalidModifier = -1;
            const int userid = testUserId;
            const int postId = 1760;

            var resultAdd = service.Add(userid, postId, true);

            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid, postId * invalidModifier));

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Get(userid, postId).Id);
        }

        [Fact]
        public void HistoryDeleteBookmarkInvalidUser()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int invalidModifier = -1;
            const int userid = testUserId;
            const int postId = 709;

            var resultAdd = service.Add(userid, postId, true);

            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid * invalidModifier, postId));

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Get(userid, postId).Id);
        }

        [Fact]
        public void HistoryDeleteBookmarkInvalidUserAndPost()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int invalidModifier = -1;
            const int userid = testUserId;
            const int postId = 1711;

            var resultAdd = service.Add(userid, postId, true);

            Assert.True(resultAdd);
            Assert.False(service.DeleteBookmark(userid * invalidModifier, postId * invalidModifier));

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Get(userid, postId).Id);
        }

        [Fact]
        public void HistoryDeleteBookmarkValid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int userid = testUserId;
            const int postId = 1760;

            var resultAdd = service.Add(userid, postId, true);

            Assert.True(resultAdd);
            
            var resultDelete = service.DeleteBookmark(userid, postId);
            Assert.True(resultDelete);

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Get(userid, postId).Id);
        }

        [Fact]
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


        [Fact]
        public void HistoryDeleteUserHistory()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userid = testUserId;

            const int postId1 = 19;
            const int postId2 = 709;
            const int postId3 = 1760;
            const int postId4 = 1711;

            var addResult1 = service.Add(userid, postId1, false);
            var addResult2 = service.Add(userid, postId2, false);
            var addResult3 = service.Add(userid, postId3, true);
            var addResult4 = service.Add(userid, postId4, false);

            var historyPre = service.GetHistoryList(userid);
            var historyDeletion = service.DeleteUserHistory(userid);
            var historyPost = service.GetHistoryList(userid);

            //clean up todo delete when mock is working
            service.DeleteHistory(service.Get(userid, postId3).Id);
            //end of clean up

            Assert.True(addResult1);
            Assert.True(addResult2);
            Assert.True(addResult3);
            Assert.True(addResult4);

            Assert.Equal(3, historyPre.Count);
            Assert.True(historyDeletion);
            Assert.Empty(historyPost);
        }

        [Fact]
        public void HistoryDeleteInvalid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int userId = -5;

            Assert.False(service.DeleteHistory(userId));
        }

        [Fact]
        public void HistoryDeleteValid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userId = testUserId;
            const int postId = 709;
            const bool isBookmark = true;
            var historyToAdd = new History {UserId = userId, PostId = postId, IsBookmark = isBookmark};

            var resultAdd = service.Add(historyToAdd);
            var history = service.Get(userId, postId);

            Assert.True(resultAdd);
            Assert.True(service.HistoryExist(history.Id));
            Assert.True(service.DeleteHistory(history.Id));
            Assert.False(service.HistoryExist(history.Id));
        }

        [Fact]
        public void HistoryExistFalse()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int historyId = -8; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.HistoryExist(historyId));
        }

        [Fact]
        public void HistoryExistTrue()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int historyId = 11; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.HistoryExist(historyId));
        }

        [Fact]
        public void HistoryGetInvalid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int id = -31;

            var history = service.Get(id);

            Assert.Null(history);
        }

        [Fact]
        public void HistoryGetInvalid2()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userId = -31;
            const int postId = -123;

            var history = service.Get(userId, postId);

            Assert.Null(history);
        }

        [Fact]
        public void GetHistoryList()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userId = testUserId;

            const int postId1 = 19;
            const int postId2 = 709;
            const int postId3 = 1760;

            var addResult1 = service.Add(userId, postId1, false);
            var addResult2 = service.Add(userId, postId2, true);
            var addResult3 = service.Add(userId, postId3, false);
            var history = service.GetHistoryList(userId);


            //clean up todo delete when mock is working
            service.DeleteHistory(service.Get(userId, postId1).Id);
            service.DeleteHistory(service.Get(userId, postId2).Id);
            service.DeleteHistory(service.Get(userId, postId3).Id);
            //end of clean up            

            Assert.True(addResult1);
            Assert.True(addResult2);
            Assert.True(addResult3);

            Assert.Equal(2, history.Count);
            Assert.Equal(postId3, history[1].PostId);
            Assert.Equal(postId1, history[0].PostId);
        }

        [Fact]
        public void GetBookmarks()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();
            const int userId = testUserId;

            const int postId1 = 19;
            const int postId2 = 709;
            const int postId3 = 1760;
            const int postId4 = 1711;

            var addResult1 = service.Add(userId, postId1, false);
            var addResult2 = service.Add(userId, postId2, true);
            var addResult3 = service.Add(userId, postId3, true);
            var addResult4 = service.Add(userId, postId4, false);
            var history = service.GetBookmarkList(userId);


            //clean up todo delete when mock is working
            service.DeleteHistory(service.Get(userId, postId1).Id);
            service.DeleteHistory(service.Get(userId, postId2).Id);
            service.DeleteHistory(service.Get(userId, postId3).Id);
            service.DeleteHistory(service.Get(userId, postId4).Id);
            //end of clean up            

            Assert.True(addResult1);
            Assert.True(addResult2);
            Assert.True(addResult3);
            Assert.True(addResult4);

            Assert.Equal(2, history.Count);
            Assert.Equal(postId3, history[1].PostId);
            Assert.Equal(postId2, history[0].PostId);
        }

        [Fact]
        public void HistoryGetValid()
        {
            var service = _serviceProvider.GetRequiredService<IHistoryRepository>();

            const int userId = testUserId;
            const int postId = 709;
            const bool isBookmark = true;
            var history = new History {UserId = userId, PostId = postId, IsBookmark = isBookmark};

            var historyAdd = service.Add(history);
            var historyGet = service.Get(userId, postId);

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
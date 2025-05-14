using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Implementation;
using Repositories.Interfaces;
using Xunit;

namespace Tests.Infrastructure.IntegrationTests
{
    public class AppUserRepositoryRepositoryTests
    {

        private readonly IDbContextFactory<DatabaseContext2> _dbContextFactory;
        private readonly ServiceProvider _serviceProvider;
        private const int userId = 12;
        private const string userName = "in";
        private const string Password = "55";
        private const string Salt = "salty";

        public AppUserRepositoryRepositoryTests()
        {
            string database = "host=localhost;port=5432;db=stackoverflow;uid=postgres;pwd=Password123";
            var services = new ServiceCollection();
            services.AddSingleton<IUserRepository, AppUserRepositoryRepository>();
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
        public void AppUserExistByIdFalse()
        {
            IUserRepository service = _serviceProvider.GetRequiredService<IUserRepository>();
            const int nonUserId = -1; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.AppUserExist(nonUserId));
        }

        [Fact]
        public void AppUserExistByIdTrue()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            //const int userId = 12; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.AppUserExist(userId));
        }

        [Fact]
        public void AppUserExistByNameFalse()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string nonUserName = "£@£@£@€$£$£{£$£@$€$£€€£$€"; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.AppUserExist(nonUserName));
        }

        [Fact]
        public void AppUserExistByNameTrue()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            //const string userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.AppUserExist(userName));
        }

        [Fact]
        public void GetAppUserById()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            //const int userId = 12;
            //const string userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.Equal(userName, service.GetAppUserName(userId));
        }

        [Fact]
        public void GetAppUserByName()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            //const int userId = 12;
            //const string userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.Equal(userId, service.GetAppUserId(userName));
        }

        [Fact]
        public void CreateAppUser()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string newUserName = "Mr. Tester von testons1";

            bool creationBool = service.CreateAppUser(newUserName, Password, Salt);
            int newUserId = service.GetAppUserId(newUserName);

            Assert.True(creationBool);
            Assert.Equal(newUserName, service.GetAppUserName(newUserId));

            //clean up todo delete when mock is working
            service.DeleteAppUser(newUserId);
        }

        [Fact]
        public void CreateAppUserTwice()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string newUserName = "Mr. Tester von testons";

            bool creationBoolOne = service.CreateAppUser(newUserName, Password, Salt);
            bool creationBoolTwo = service.CreateAppUser(newUserName, Password, Salt);
            int newUserId = service.GetAppUserId(newUserName);

            Assert.True(creationBoolOne);
            Assert.False(creationBoolTwo);
            Assert.Equal(newUserName, service.GetAppUserName(newUserId));

            //clean up todo delete when mock is working
            service.DeleteAppUser(newUserId);
        }

        [Fact]
        public void CreateUserGetObject()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string newUserName = "Mr. Tester von testonsen";

            AppUser user = service.CreateUser(newUserName, Password, Salt);

            Assert.Equal(newUserName, user.Username);

            //clean up todo delete when mock is working
            service.DeleteAppUser(user.Id);
        }

        [Fact]
        public void CreateUserGetObjectNull()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);

            AppUser user = service.CreateUser(userName, Password, Salt);

            Assert.Null(user);
        }

        [Fact]
        public void UpdateAppUserNameValidUser()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string userNameOne = "Ms. donald docker";
            const string userNameTwo = "Ms. donald ducker";

            bool creationBool = service.CreateAppUser(userNameOne, Password, Salt);
            int userIdOne = service.GetAppUserId(userNameOne);
            bool updateBool = service.UpdateAppUserName(userNameOne, userNameTwo);
            int userIdTwo = service.GetAppUserId(userNameTwo);

            Assert.True(creationBool);
            Assert.True(updateBool);

            Assert.Equal(userIdOne, userIdTwo);

            Assert.NotEqual(userNameOne, service.GetAppUserName(userIdOne));
            Assert.Equal(userNameTwo, service.GetAppUserName(userIdOne));

            //clean up todo delete when mock is working
            service.DeleteAppUser(userIdOne);
            service.DeleteAppUser(userIdTwo);
        }

        [Fact]
        public void UpdateAppUserNameInvalidUser()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string userNameOne = "Ms. ronaldo docker";
            const string userNameTwo = "Ms. ronaldo ducker";

            bool updateBool = service.UpdateAppUserName(userNameOne, userNameTwo);

            Assert.False(updateBool);
        }

        [Fact]
        public void DeleteAppUserByNameTrue()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string newUserName = "dock";

            bool creationBool = service.CreateAppUser(newUserName, Password, Salt);
            bool existBeforeDeletion = service.AppUserExist(newUserName);
            bool deletionBool = service.DeleteAppUser(newUserName);

            Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.True(deletionBool);
            Assert.False(service.AppUserExist(newUserName));
        }

        [Fact]
        public void DeleteAppUserByNameFalse()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string newUserName = "docker";
            const string falseName = "not docker";

            bool creationBool = service.CreateAppUser(newUserName, Password, Salt);
            bool existBeforeDeletion = service.AppUserExist(newUserName);
            bool deletionBool = service.DeleteAppUser(falseName);

            Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.False(deletionBool);
            Assert.True(service.AppUserExist(newUserName));

            //clean up todo delete when mock is working
            service.DeleteAppUser(newUserName);
        }

        [Fact]
        public void DeleteAppUserByIdTrue()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string newUserName = "donald";

            bool creationBool = service.CreateAppUser(newUserName, Password, Salt);
            int newUserId = service.GetAppUserId(newUserName);
            bool existBeforeDeletion = service.AppUserExist(newUserId);
            bool deletionBool = service.DeleteAppUser(newUserId);

            Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.True(deletionBool);
            Assert.False(service.AppUserExist(newUserName));
            Assert.False(service.AppUserExist(newUserId));
        }

        [Fact]
        public void DeleteAppUserByIdFalse()
        {
            IUserRepository service = new AppUserRepositoryRepository(_dbContextFactory);
            const string newUserName = "niels";
            const int falseId = -2;

            bool creationBool = service.CreateAppUser(newUserName, Password, Salt);
            int newUserId = service.GetAppUserId(newUserName);
            bool existBeforeDeletion = service.AppUserExist(newUserId);
            bool deletionBool = service.DeleteAppUser(falseId);

            Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.False(deletionBool);
            Assert.True(service.AppUserExist(newUserName));
            Assert.True(service.AppUserExist(newUserId));

            //clean up todo delete when mock is working
            service.DeleteAppUser(newUserName);
        }
    }
}
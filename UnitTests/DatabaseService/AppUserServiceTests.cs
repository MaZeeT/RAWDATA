using DatabaseService.Modules;
using DatabaseService.Services;
using Xunit;

namespace UnitTests.DatabaseService
{
    public class AppUserServiceTests
    {
        private const int userId = 12;
        private const string userName = "in";
        private const string Password = "55";
        private const string Salt = "salty";

        [Fact]
        public void AppUserExistByIdFalse()
        {
            IAppUserService service = new AppUserService();
            const int nonUserId = -1; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.AppUserExist(nonUserId));
        }

        [Fact]
        public void AppUserExistByIdTrue()
        {
            IAppUserService service = new AppUserService();
            //const int userId = 12; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.AppUserExist(userId));
        }

        [Fact]
        public void AppUserExistByNameFalse()
        {
            IAppUserService service = new AppUserService();
            const string nonUserName = "£@£@£@€$£$£{£$£@$€$£€€£$€"; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.AppUserExist(nonUserName));
        }

        [Fact]
        public void AppUserExistByNameTrue()
        {
            IAppUserService service = new AppUserService();
            //const string userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.AppUserExist(userName));
        }

        [Fact]
        public void GetAppUserById()
        {
            IAppUserService service = new AppUserService();
            //const int userId = 12;
            //const string userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.Equal(userName, service.GetAppUserName(userId));
        }

        [Fact]
        public void GetAppUserByName()
        {
            IAppUserService service = new AppUserService();
            //const int userId = 12;
            //const string userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.Equal(userId, service.GetAppUserId(userName));
        }

        [Fact]
        public void CreateAppUser()
        {
            IAppUserService service = new AppUserService();
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
            IAppUserService service = new AppUserService();
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
            IAppUserService service = new AppUserService();
            const string newUserName = "Mr. Tester von testonsen";

            AppUser user = service.CreateUser(newUserName, Password, Salt);

            Assert.Equal(newUserName, user.Username);

            //clean up todo delete when mock is working
            service.DeleteAppUser(user.Id);
        }

        [Fact]
        public void CreateUserGetObjectNull()
        {
            IAppUserService service = new AppUserService();

            AppUser user = service.CreateUser(userName, Password, Salt);

            Assert.Null(user);
        }

        [Fact]
        public void UpdateAppUserNameValidUser()
        {
            IAppUserService service = new AppUserService();
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
            IAppUserService service = new AppUserService();
            const string userNameOne = "Ms. ronaldo docker";
            const string userNameTwo = "Ms. ronaldo ducker";

            bool updateBool = service.UpdateAppUserName(userNameOne, userNameTwo);

            Assert.False(updateBool);
        }

        [Fact]
        public void DeleteAppUserByNameTrue()
        {
            IAppUserService service = new AppUserService();
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
            IAppUserService service = new AppUserService();
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
            IAppUserService service = new AppUserService();
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
            IAppUserService service = new AppUserService();
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
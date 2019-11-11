using DatabaseService.Services;
using Xunit;

namespace UnitTests.DatabaseService
{
    public class AppUserServiceTests
    {
        private string Password = "55";
        private string Salt = "salty";
        
        [Fact]
        public void AppUserExistByIdFalse()
        {
            var service = new AppUserService();
            var userId = -1; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.AppUserExist(userId));
        }

        [Fact]
        public void AppUserExistByIdTrue()
        {
            IAppUserService service = new AppUserService();
            var userId = 0; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.AppUserExist(userId));
        }

        [Fact]
        public void AppUserExistByNameFalse()
        {
            IAppUserService service = new AppUserService();
            var userName = "£@£@£@€$£$£{£$£@$€$£€€£$€"; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.AppUserExist(userName));
        }

        [Fact]
        public void AppUserExistByNameTrue()
        {
            IAppUserService service = new AppUserService();
            var userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.AppUserExist(userName));
        }

        [Fact]
        public void GetAppUserById()
        {
            IAppUserService service = new AppUserService();
            var userId = 0;
            var userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.Equal(userName, service.GetAppUserName(userId));
        }

        [Fact]
        public void GetAppUserByName()
        {
            IAppUserService service = new AppUserService();
            var userId = 0;
            var userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.Equal(userId, service.GetAppUserId(userName));
        }

        [Fact]
        public void CreateAppUser()
        {
            IAppUserService service = new AppUserService();
            var userName = "Mr. Tester von testons";

            var creationBool = service.CreateAppUser(userName,Password,Salt);
            var userId = service.GetAppUserId(userName);

            Assert.True(creationBool);
            Assert.Equal(userName, service.GetAppUserName(userId));
            
            //clean up todo delete when mock is working
            service.DeleteAppUser(userId);
        }
        
        [Fact]
        public void CreateAppUserTwice()
        {
            IAppUserService service = new AppUserService();
            var userName = "Mr. Tester von testons";

            var creationBoolOne = service.CreateAppUser(userName,Password,Salt);
            var creationBoolTwo = service.CreateAppUser(userName,Password,Salt);
            var userId = service.GetAppUserId(userName);

            Assert.True(creationBoolOne);
            Assert.False(creationBoolTwo);
            Assert.Equal(userName, service.GetAppUserName(userId));
            
            //clean up todo delete when mock is working
            service.DeleteAppUser(userId);
        }
        
        [Fact]
        public void CreateUserGetObject()
        {
            IAppUserService service = new AppUserService();
            var userName = "Mr. Tester von testonsen";

            var user = service.CreateUser(userName,Password,Salt);
            
            Assert.Equal(userName, user.Username);
            
            //clean up todo delete when mock is working
            service.DeleteAppUser(user.Id);
        }
        
        [Fact]
        public void CreateUserGetObjectNull()
        {
            IAppUserService service = new AppUserService();
            var userName = "in";

            var user = service.CreateUser(userName,Password,Salt);
            
            Assert.Null(user);
        }

        [Fact]
        public void UpdateAppUserNameValidUser()
        {
            IAppUserService service = new AppUserService();
            var userNameOne = "Ms. donald docker";
            var userNameTwo = "Ms. donald ducker";

            var creationBool = service.CreateAppUser(userNameOne,Password,Salt);
            var userIdOne = service.GetAppUserId(userNameOne);
            var updateBool = service.UpdateAppUserName(userNameOne, userNameTwo);
            var userIdTwo = service.GetAppUserId(userNameTwo);

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
            var userNameOne = "Ms. ronaldo docker";
            var userNameTwo = "Ms. ronaldo ducker";
            
            var updateBool = service.UpdateAppUserName(userNameOne, userNameTwo);

            Assert.False(updateBool);
        }

        [Fact]
        public void DeleteAppUserByNameTrue()
        {
            IAppUserService service = new AppUserService();
            var userName = "dock";

            var creationBool = service.CreateAppUser(userName,Password,Salt);
            var existBeforeDeletion = service.AppUserExist(userName);
            var deletionBool = service.DeleteAppUser(userName);

            Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.True(deletionBool);
            Assert.False(service.AppUserExist(userName));
        }
        
        [Fact]
        public void DeleteAppUserByNameFalse()
        {
            IAppUserService service = new AppUserService();
            var userName = "docker";
            var falseName = "not docker";

            var creationBool = service.CreateAppUser(userName,Password,Salt);
            var existBeforeDeletion = service.AppUserExist(userName);
            var deletionBool = service.DeleteAppUser(falseName);

            Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.False(deletionBool);
            Assert.True(service.AppUserExist(userName));
            
            //clean up todo delete when mock is working
            service.DeleteAppUser(userName);
        }
        
        [Fact]
        public void DeleteAppUserByIdTrue()
        {
            IAppUserService service = new AppUserService();
            var userName = "donald";

            var creationBool = service.CreateAppUser(userName,Password,Salt);
            var userId = service.GetAppUserId(userName);
            var existBeforeDeletion = service.AppUserExist(userId);
            var deletionBool = service.DeleteAppUser(userId);

            Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.True(deletionBool);
            Assert.False(service.AppUserExist(userName));
            Assert.False(service.AppUserExist(userId));
        }
        
        [Fact]
        public void DeleteAppUserByIdFalse()
        {
            IAppUserService service = new AppUserService();
            var userName = "niels";
            var falseId = -2;

            var creationBool = service.CreateAppUser(userName,Password,Salt);
            var userId = service.GetAppUserId(userName);
            var existBeforeDeletion = service.AppUserExist(userId);
            var deletionBool = service.DeleteAppUser(falseId);

            Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.False(deletionBool);
            Assert.True(service.AppUserExist(userName));
            Assert.True(service.AppUserExist(userId));
            
            //clean up todo delete when mock is working
            service.DeleteAppUser(userName);
        }
        
    }
}
using DatabaseService.Services;
using Xunit;

namespace UnitTests
{
    public class AppUserServiceTests
    {
        [Fact]
        public void AppUserExistByIdFalse()
        {
            var service = new AppUsersService();
            var userId = -1; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.AppUserExist(userId));
        }

        [Fact]
        public void AppUserExistByIdTrue()
        {
            var service = new AppUsersService();
            var userId = 0; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.AppUserExist(userId));
        }

        [Fact]
        public void AppUserExistByNameFalse()
        {
            var service = new AppUsersService();
            var userName = "£@£@£@€$£$£{£$£@$€$£€€£$€"; //Hardcoded user in DB //todo replace with a mock

            Assert.False(service.AppUserExist(userName));
        }

        [Fact]
        public void AppUserExistByNameTrue()
        {
            var service = new AppUsersService();
            var userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.True(service.AppUserExist(userName));
        }

        [Fact]
        public void GetAppUserById()
        {
            var service = new AppUsersService();
            var userId = 0;
            var userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.Equal(userName, service.GetAppUserName(userId));
        }

        [Fact]
        public void GetAppUserByName()
        {
            var service = new AppUsersService();
            var userId = 0;
            var userName = "in"; //Hardcoded user in DB //todo replace with a mock

            Assert.Equal(userId, service.GetAppUserId(userName));
        }

        [Fact]
        public void CreateAppUser()
        {
            var service = new AppUsersService();
            var userName = "Mr. Tester von testons";

            var creationBool = service.CreateAppUser(userName);
            var userId = service.GetAppUserId(userName);

            Assert.True(creationBool);
            Assert.Equal(userName, service.GetAppUserName(userId));
            
            //clean up todo delete when mock is working
            service.DeleteAppUser(userId);
        }
        
        [Fact]
        public void CreateAppUserTwice()
        {
            var service = new AppUsersService();
            var userName = "Mr. Tester von testons";

            var creationBoolOne = service.CreateAppUser(userName);
            var creationBoolTwo = service.CreateAppUser(userName);
            var userId = service.GetAppUserId(userName);

            Assert.True(creationBoolOne);
            Assert.False(creationBoolTwo);
            Assert.Equal(userName, service.GetAppUserName(userId));
            
            //clean up todo delete when mock is working
            service.DeleteAppUser(userId);
        }

        [Fact]
        public void UpdateAppUserNameValidUser()
        {
            var service = new AppUsersService();
            var userNameOne = "Ms. donald docker";
            var userNameTwo = "Ms. donald ducker";

            var creationBool = service.CreateAppUser(userNameOne);
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
            var service = new AppUsersService();
            var userNameOne = "Ms. ronaldo docker";
            var userNameTwo = "Ms. ronaldo ducker";
            
            var updateBool = service.UpdateAppUserName(userNameOne, userNameTwo);

            Assert.False(updateBool);
        }

        [Fact]
        public void DeleteAppUserByNameTrue()
        {
            var service = new AppUsersService();
            var userName = "dock";

            var creationBool = service.CreateAppUser(userName);
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
            var service = new AppUsersService();
            var userName = "docker";
            var falseName = "not docker";

            var creationBool = service.CreateAppUser(userName);
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
            var service = new AppUsersService();
            var userName = "donald";

            var creationBool = service.CreateAppUser(userName);
            var userId = service.GetAppUserId(userName);
            var existBeforeDeletion = service.AppUserExist(userId);
            var deletionBool = service.DeleteAppUser(userId);

            Assert.True(creationBool);
            //Assert.True(existBeforeDeletion);
            Assert.True(deletionBool);
            Assert.False(service.AppUserExist(userName));
            Assert.False(service.AppUserExist(userId));
        }
        
        [Fact]
        public void DeleteAppUserByIdFalse()
        {
            var service = new AppUsersService();
            var userName = "niels";
            var falseId = -2;

            var creationBool = service.CreateAppUser(userName);
            var userId = service.GetAppUserId(userName);
            var existBeforeDeletion = service.AppUserExist(userId);
            var deletionBool = service.DeleteAppUser(falseId);

            //Assert.True(creationBool);
            Assert.True(existBeforeDeletion);
            Assert.False(deletionBool);
            Assert.True(service.AppUserExist(userName));
            Assert.True(service.AppUserExist(userId));
            
            //clean up todo delete when mock is working
            service.DeleteAppUser(userName);
        }
        
    }
}
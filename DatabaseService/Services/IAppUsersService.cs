using DatabaseService.Modules;

namespace DatabaseService.Services
{
    public interface IAppUsersService
    {
        bool CreateAppUser(string username, string password, string salt);
         bool UpdateAppUserName(string oldName, string newName);
         bool DeleteAppUser(int id);
         bool DeleteAppUser(string username);
        string GetAppUserName(int id);
        bool AppUserExist(int id);
        bool AppUserExist(string username);
        int GetAppUserId(string username);
    }
}
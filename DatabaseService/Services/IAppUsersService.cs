using DatabaseService.Modules;

namespace DatabaseService.Services
{
    public interface IAppUsersService
    {
        //todo suggestions to the interface
         bool CreateAppUser(string name);
         bool UpdateAppUserName(string oldName, string newName);
         bool DeleteAppUser(int id);
         bool DeleteAppUser(string username);
        string GetAppUserName(int id);
        bool AppUserExist(int id);
        bool AppUserExist(string username);
        int GetAppUserId(string username);
    }
}
using DatabaseService.Modules;

namespace DatabaseService.Interfaces.Repositories;
public interface IUser
{
    bool CreateAppUser(string username, string password, string salt);
    AppUser CreateUser(string name, string password, string salt);
    bool UpdateAppUserName(string oldName, string newName);
    bool DeleteAppUser(int id);
    bool DeleteAppUser(string username);
    AppUser GetAppUser(string username);
    string GetAppUserName(int id);
    int GetAppUserId(string username);
    bool AppUserExist(int id);
    bool AppUserExist(string username);
}
using DatabaseService.Modules;

namespace DatabaseService.Services
{
    public interface IAppUsersService
    {
        //todo suggestions to the interface
        // bool CreateAppUser(string name);
        // bool UpdateAppUser();
        // bool DeleteAppUser(int id);
        // bool DeleteAppUser(string name);
        string GetAppUser(int id);
        // AppUser GetAppUser(string name);
    }
}
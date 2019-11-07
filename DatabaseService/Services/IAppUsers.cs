namespace DatabaseService.Services
{
    public interface IAppUsers
    {
        bool CreateAppUser(string name);
        bool UpdateAppUser();
        bool DeleteAppUser(int id);
        bool DeleteAppUser(string name);
        AppUserses GetAppUser(int id);
        AppUserses GetAppUser(string name);
    }
}
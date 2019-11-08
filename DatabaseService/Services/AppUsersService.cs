using DatabaseService.Modules;

namespace DatabaseService.Services
{
    public class AppUsersService : IAppUsersService
    {
        public string GetAppUser(int id)
        {
            using var database = new AppContext();
            //return new AppUser(id, "test");
            return database.AppUser.Find(id).name;

        }

        /*
        public AppUser GetAppUser(string name)
        {
            using var database = new AppContext();
            return database.AppUser.Find(name);
        }
        */
        
    }
}
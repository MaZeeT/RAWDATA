using DatabaseService.Modules;

namespace DatabaseService.Services
{
    public class AppUsersService : IAppUsersService
    {
        public AppUser GetAppUser(int id)
        {
            using var database = new AppContext();
            //return new AppUser(id, "test");
            return database.AppUser.Find(id);

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
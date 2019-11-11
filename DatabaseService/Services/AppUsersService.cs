using System.Linq;
using DatabaseService.Modules;

namespace DatabaseService.Services
{

    public class AppUsersService : IAppUsersService
    {
         AppContext database;

         public AppUsersService()
         {
             database = new AppContext();
         }
        
        public string GetAppUserName(int id)
        {
            var result = database.AppUser.Find(id);
            return result.name;
        }

        public int GetAppUserId(string username)
        {
            var appUsers = database.AppUser.Where(user => user.name == username).ToList();
            if (appUsers.Count > 0)
            {
                return appUsers.First().id;
            }

            return -1;
        }

        public bool CreateAppUser(string username)
        {
            if (!AppUserExist(username))
            {
                database.AppUser.Add(new AppUser() {name = username});

                var result = database.SaveChanges();
                return result > 0;
            }

            return false;
        }

        public bool UpdateAppUserName(string oldName, string newName)
        {
            if (AppUserExist(oldName))
            {
                int appUserId = GetAppUserId(oldName);
                var appUser = database.AppUser.Find(appUserId);
                database.AppUser.Update(appUser);
                appUser.name = newName;
                var result = database.SaveChanges();
                return result > 0;
            }

            return false;
        }

        public bool DeleteAppUser(int id)
        {
            if (AppUserExist(id))
            {
                var appUser = database.AppUser.Find(id);
                database.AppUser.Remove(appUser);

                var result = database.SaveChanges();
                return result > 0;
            }

            return false;
        }

        public bool DeleteAppUser(string username)
        {
            return DeleteAppUser(GetAppUserId(username));
        }

        public bool AppUserExist(int id)
        {
            var result = database.AppUser.Find(id);
            return result != null;
        }

        public bool AppUserExist(string username)
        {
            return AppUserExist(GetAppUserId(username));
        }
        
    }
}
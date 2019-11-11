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
            var result = database.AppUsers.Find(id);
            return result.Username;
        }

        public int GetAppUserId(string username)
        {
            var appUsers = database.AppUsers.Where(user => user.Username == username).ToList();
            if (appUsers.Count > 0)
            {
                return appUsers.First().Id;
            }

            return -1;
        }

        public bool CreateAppUser(string username)
        {
            if (!AppUserExist(username))
            {
                database.AppUsers.Add(new AppUsers() {Username = username});

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
                var appUser = database.AppUsers.Find(appUserId);
                database.AppUsers.Update(appUser);
                appUser.Username = newName;
                var result = database.SaveChanges();
                return result > 0;
            }

            return false;
        }

        public bool DeleteAppUser(int id)
        {
            if (AppUserExist(id))
            {
                var appUser = database.AppUsers.Find(id);
                database.AppUsers.Remove(appUser);

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
            var result = database.AppUsers.Find(id);
            return result != null;
        }

        public bool AppUserExist(string username)
        {
            return AppUserExist(GetAppUserId(username));
        }
        
    }
}
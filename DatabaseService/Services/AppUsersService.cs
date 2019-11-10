using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DatabaseService.Modules;

namespace DatabaseService.Services
{
    public class AppUsersService : IAppUsersService
    {
        private bool DatabaseModify(string query)
        {
            using var database = new AppContext();
            database.AppUser.FromSqlRaw(query);
            return true; //todo return bool depending on DB not hardcode
        }

        public string GetAppUserName(int id)
        {
            if (AppUserExist(id))
            {
                using var database = new AppContext();
                var result = database.AppUser.Find(id);
                return result.name;
            }
            else
            {
                return null;
            }
        }

        public int GetAppUserId(string username)
        {
            if (AppUserExist(username))
            {
                using var database = new AppContext();
                var appUser = database.AppUser.First(user => user.name == username);
                return appUser.id;
            }
            else
            {
                return -1;
            }
        }

        public bool CreateAppUser(string username)
        {
            if (AppUserExist(username))
            {
                return false;
            }
            else
            {
                using var database = new AppContext();
                database.AppUser.Add(new AppUser() {name = username});

                var result = database.SaveChanges();
                Console.WriteLine(result); //todo remove
                return result > 0;
            }
        }

        public bool UpdateAppUserName(string oldName, string newName)
        {
            if (AppUserExist(oldName))
            {
                using var database = new AppContext();
                int appUserId = GetAppUserId(oldName);
                var appUser = database.AppUser.Find(appUserId);
                database.AppUser.Update(appUser);
                appUser.name = newName;
                var result = database.SaveChanges();
                return result > 0;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAppUser(int id)
        {
            if (AppUserExist(id))
            {
                using var database = new AppContext();
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
            using var database = new AppContext();
            var result = database.AppUser.Find(id);
            return result != null;
        }

        public bool AppUserExist(string username)
        {
            return AppUserExist(GetAppUserId(username));
        }
    }
}
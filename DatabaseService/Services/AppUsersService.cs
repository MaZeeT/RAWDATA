using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
                //return new AppUser(id, "test");
                return database.AppUser.Find(id).name;
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
                string query = "SELECT id FROM appusers WHERE appusers.name = 'in';";
                
                using var database = new AppContext();
                var result = database.AppUser.FromSqlRaw(query);
                return Convert.ToInt32(result.FirstOrDefault());
            }
            else
            {
                return -1;
            }
        }

        public bool CreateAppUser(string name)
        {
            if (AppUserExist(name))
            {
                return false;
            }
            else
            {
                var query = "INSERT INTO appusers (name) VALUES ('{name}');";
                return DatabaseModify(query);
            }
        }

        public bool UpdateAppUserName(string oldName, string newName)
        {
            if (AppUserExist(oldName))
            {
                var query = "UPDATE appusers SET name = '{newName}' WHERE appusers.name ='{oldName}}';";
                return DatabaseModify(query);
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
                var query = "DELETE FROM appusers WHERE appusers.id = {id};";
                return DatabaseModify(query);
            }
            return false;
        }

        public bool DeleteAppUser(string username)
        {
            return DeleteAppUser(GetAppUserId(username));
        }

        public bool AppUserExist(int id)
        {
            string query = "SELECT COUNT(id) FROM appusers WHERE appusers.id = '{id}}';";
                
            using var database = new AppContext();
            var result = database.AppUser.FromSqlRaw(query);
            var cResult = Convert.ToInt32(result.FirstOrDefault());
            if (cResult == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AppUserExist(string username)
        {
            return AppUserExist(GetAppUserId(username));
        }
        
    }
}
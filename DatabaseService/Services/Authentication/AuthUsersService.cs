using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseService.Modules;

namespace DatabaseService.Services.Authentication
{
    public class AuthUsersService : IAuthUsersService
    {
        public AppUsers GetUserByUserName(string username)
        {
            using var DB = new AppContext();
            var user = DB.AppUsers.Where(u => u.Username == username)
                                  .FirstOrDefault();


            return user;
        }

        public AppUsers CreateUser(string username, string password, string salt)
            //
        {
            var user = new AppUsers()
            {
               Username = username,
               Password = password,
               Salt = salt
            };
            using var DB = new AppContext();
            DB.AppUsers.Add(user);
            DB.SaveChanges();

            var newlyAddedUser = GetUserByUserName(user.Username);
            return newlyAddedUser;
        }

    }
}

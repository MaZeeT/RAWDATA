using DatabaseService.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseService.Services
{
    public interface IAuthUsersService
    {
        AppUsers GetUserByUserName(string username);
        AppUsers CreateUser(string username, string password, string salt);
    }
}

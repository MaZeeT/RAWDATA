using DatabaseService.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseService.Services
{
    public interface IAuthUsersService
    {
        AuthUsers GetUserByUserName(string username);
        AuthUsers CreateUser(string username, string password, string salt);
    }
}

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using DatabaseService.Modules;

namespace DatabaseService.Services
{
    public class AppUsersService : IAppUsersService
    {
        public AppUser GetAppUser(int id)
        {
            using var Database = new AppContext();
            
            
            
            
            
           return new AppUser();
        }

        public AppUser GetAppUser(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
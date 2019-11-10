using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseService.Modules
{
    public class AuthUsers
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}

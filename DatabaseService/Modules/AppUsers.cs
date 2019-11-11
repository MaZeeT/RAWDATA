using System.Net.Sockets;

namespace DatabaseService.Modules
{
    public class AppUsers
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
using System.Net.Sockets;

namespace DatabaseService.Modules
{
    public class AppUser
    {
        public int id { set; get; }
        public string name { set; get; }


        public AppUser()
        {
            
        }
        
        public AppUser(int id)
        {
            this.id = id;
        }
        
        public AppUser(string name)
        {
            this.name = name;
        }
        
        public AppUser(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        
    }
}
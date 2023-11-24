namespace DatabaseService.Modules
{
    public class AppUser
    {
        public int Id { get; }
        
        public string Username { set; get; }
        
        public string Password { get; set; }
        
        public string Salt { get; set; }
        
    }
}
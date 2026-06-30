namespace Domain.Entities;

public class AppUser
{
    public int Id { get; set; }
        
    public string Username { get; set; }
        
    public string Password { get; set; }
        
    public string Salt { get; set; }
        
}
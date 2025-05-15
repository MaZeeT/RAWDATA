using Domain.Entities;

namespace BusinessLogic.Interfaces;

public interface IUserService
{
    string GetUserName(int id);
    AppUser GetAppUser(string username);
    AppUser CreateUser(string name, string password, string salt);
}

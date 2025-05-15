using BusinessLogic.Interfaces;
using Domain.Entities;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string GetUserName(int id)
    {
        return _userRepository.GetAppUserName(id);
    }

    public AppUser GetAppUser(string username)
    {
        return _userRepository.GetAppUser(username);
    }

    public AppUser CreateUser(string name, string password, string salt)
    {
        return _userRepository.CreateUser(name, password, salt);
    }
}

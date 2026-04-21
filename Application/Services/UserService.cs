using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string? GetUserName(int id)
    {
        return _userRepository.GetAppUserName(id);
    }

    public AppUser? GetAppUser(string username)
    {
        return _userRepository.GetAppUser(username);
    }

    public AppUser CreateUser(string name, string password, string salt)
    {
        return _userRepository.CreateUser(name, password, salt);
    }

    public bool UserExists(string username)
    {
        return _userRepository.AppUserExist(username);
    }
}

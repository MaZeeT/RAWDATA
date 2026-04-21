using Application.Common;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserService(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
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

    public Result<AppUser> CreateUser(string username, string password, string salt)
    {
        var user = new AppUser
        {
            Username = username,
            Password = password,
            Salt = salt
        };

        if (_userRepository.AppUserExist(user))
        {
            return Result<AppUser>.Failure("User with the same name already exists");
        }
        
        var appUser = _userRepository.Add(user);
        
        _unitOfWork.Commit();

        return Result<AppUser>.Success(appUser);
    }

    public bool UserExists(string username)
    {
        var user = new AppUser
        {
            Username = username
        };
        
        return _userRepository.AppUserExist(user);
    }
}

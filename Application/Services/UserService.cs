using System.Text.RegularExpressions;
using Application.Common;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Use_Cases.CreateUser;
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

    public Result<CreateUserResult> CreateUser(CreateUserCommand createUserCommand, AuthSettings authSettings)
    {
        if (!IsValidUserCredential(createUserCommand))
        {
            return Result<CreateUserResult>.Failure("Credentials are not valid");
        }
        
        if (UserExists(createUserCommand.Username))
        {
            return Result<CreateUserResult>.Failure("Username already exists");
        }

        var salt = PasswordService.GenerateSalt(authSettings.PasswordSize);

        var pwd = PasswordService.HashPassword(createUserCommand.Password, salt, authSettings.PasswordSize);
        
        var user = new AppUser
        {
            Username = createUserCommand.Username,
            Password = pwd,
            Salt = salt
        };

        if (_userRepository.AppUserExist(user))
        {
            return Result<CreateUserResult>.Failure("User with the same name already exists");
        }
        
        var appUser = _userRepository.Add(user);
        
        _unitOfWork.Commit();

        var createUserResult = new CreateUserResult { Username = appUser.Username };

        return Result<CreateUserResult>.Success(createUserResult);
    }

    public bool UserExists(string username)
    {
        var user = new AppUser
        {
            Username = username
        };
        
        return _userRepository.AppUserExist(user);
    }
    
    private static bool IsValidUserCredential(CreateUserCommand dto)
    {
        var isUsernameOrPasswordEmpty = string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password);
        if (isUsernameOrPasswordEmpty)
        {
            return false;
        }

        var isUsernameOrPasswordToShort = dto.Username.Length < 2 || dto.Password.Length < 6;
        if (isUsernameOrPasswordToShort)
        {
            return false;
        }

        const string regExUsernameInvalidValue = @"[^a-zA-Z\d]";
        const string regExPasswordInvalidValue = @"[^a-zA-Z\d]";
        var regExMatchInvalidUser = Regex.Match(dto.Username, regExUsernameInvalidValue, RegexOptions.IgnoreCase);
        var regExMatchInvalidPassword = Regex.Match(dto.Password, regExPasswordInvalidValue, RegexOptions.IgnoreCase);

        if (!regExMatchInvalidUser.Success &&
            !regExMatchInvalidPassword.Success)
        {
            return true;
        }
        
        Console.WriteLine("Am entering successfully :D ");
        return false;
    }
}

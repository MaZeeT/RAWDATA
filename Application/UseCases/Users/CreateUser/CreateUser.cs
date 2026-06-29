using System.Text.RegularExpressions;
using Application.Common;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;

namespace Application.UseCases.Users.CreateUser;

public class CreateUser : ICreateUser
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public CreateUser(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public Result<CreateUserResult> Execute(CreateUserCommand createUserCommand, AuthSettings authSettings)
    {
        if (!IsValidUserCredential(createUserCommand.Username, createUserCommand.Password))
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
    
    private bool UserExists(string username)
    {
        var user = new AppUser
        {
            Username = username
        };

        return _userRepository.AppUserExist(user);
    }
    
    private static bool IsValidUserCredential(string username, string password)
    {
        var isUsernameOrPasswordEmpty = string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password);
        if (isUsernameOrPasswordEmpty)
        {
            return false;
        }

        var isUsernameOrPasswordToShort = username.Length < 2 || password.Length < 6;
        if (isUsernameOrPasswordToShort)
        {
            return false;
        }

        const string regExUsernameInvalidValue = @"[^a-zA-Z\d]";
        const string regExPasswordInvalidValue = @"[^a-zA-Z\d]";
        var regExMatchInvalidUser = Regex.Match(username, regExUsernameInvalidValue, RegexOptions.IgnoreCase);
        var regExMatchInvalidPassword = Regex.Match(password, regExPasswordInvalidValue, RegexOptions.IgnoreCase);

        if (!regExMatchInvalidUser.Success &&
            !regExMatchInvalidPassword.Success)
        {
            return true;
        }

        Console.WriteLine("Am entering successfully :D ");
        return false;
    }
}
using System.Text.RegularExpressions;
using Application.Common;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.UseCases.Users.CreateUser;
using Application.UseCases.Users.LoginUser;
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

    private AppUser? GetAppUser(string username)
    {
        return _userRepository.GetAppUser(username);
    }

    public Result<LoginUserResult> LoginUser(LoginUserCommand loginUserCommand, AuthSettings authSettings)
    {
        if (!IsValidUserCredential(loginUserCommand.Username, loginUserCommand.Password))
        {
            return Result<LoginUserResult>.Failure("Credentials are not valid");
        }

        var user = GetAppUser(loginUserCommand.Username);

        if (user is null || IsInvalidPassword(loginUserCommand, user, authSettings))
        {
            return Result<LoginUserResult>.Failure("Password is not valid");
        }

        var loginUserResult = new LoginUserResult
        {
            UserId = user.Id,
            UserName = user.Username
        };

        return Result<LoginUserResult>.Success(loginUserResult);
    }

    private static bool IsInvalidPassword(LoginUserCommand loginUserCommand, AppUser user, AuthSettings authSettings)
    {
        var pwd = PasswordService.HashPassword(loginUserCommand.Password, user.Salt, authSettings.PasswordSize);

        return user.Password != pwd;
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
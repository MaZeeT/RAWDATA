using System.Text.RegularExpressions;
using Application.Common;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.Users.CreateUser;

public class CreateUser : ICreateUser
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly int _passwordSize;

    public CreateUser(IUnitOfWork unitOfWork, IUserRepository userRepository, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _passwordSize = configuration.GetValue<int>("Auth:PwdSize");
    }

    public Result<CreateUserResult> Execute(CreateUserCommand createUserCommand)
    {
        if (!IsValidUserCredential(createUserCommand.Username, createUserCommand.Password))
        {
            return Result<CreateUserResult>.Failure("Credentials are not valid");
        }

        if (UserExists(createUserCommand.Username))
        {
            return Result<CreateUserResult>.Failure("Username already exists");
        }

        var salt = PasswordService.GenerateSalt(_passwordSize);

        var pwd = PasswordService.HashPassword(createUserCommand.Password, salt, _passwordSize);

        var user = new AppUser
        {
            Username = createUserCommand.Username,
            Password = pwd,
            Salt = salt
        };

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

        return false;
    }
}
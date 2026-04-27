using Application.Common;
using Application.Use_Cases.CreateUser;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IUserService
{
    string? GetUserName(int id);
    AppUser? GetAppUser(string username);
    Result<CreateUserResult> CreateUser(CreateUserCommand createUserCommand, AuthSettings authSettings);
    bool UserExists(string username);
}

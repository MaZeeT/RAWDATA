using Application.Common;
using Application.UseCases.Users.CreateUser;
using Application.UseCases.Users.LoginUser;

namespace Application.Interfaces.Services;

public interface IUserService
{
    string? GetUserName(int id);
    Result<LoginUserResult> LoginUser(LoginUserCommand loginUserCommand, AuthSettings authSettings);
}
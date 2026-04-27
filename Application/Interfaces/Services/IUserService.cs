using Application.Common;
using Application.Use_Cases.CreateUser;
using Application.Use_Cases.LoginUser;

namespace Application.Interfaces.Services;

public interface IUserService
{
    string? GetUserName(int id);
    Result<CreateUserResult> CreateUser(CreateUserCommand createUserCommand, AuthSettings authSettings);
    Result<LoginUserResult> LoginUser(LoginUserCommand loginUserCommand, AuthSettings authSettings);
}
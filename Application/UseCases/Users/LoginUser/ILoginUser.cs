using Application.Common;

namespace Application.UseCases.Users.LoginUser;

public interface ILoginUser
{
    Result<LoginUserResult> Execute(LoginUserCommand loginUserCommand);
}
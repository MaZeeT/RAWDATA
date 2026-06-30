using Application.Common;

namespace Application.UseCases.Users.GetUser;

public interface IGetUser
{
    Result<GetUserResult> Execute(GetUserCommand getUserCommand);
}
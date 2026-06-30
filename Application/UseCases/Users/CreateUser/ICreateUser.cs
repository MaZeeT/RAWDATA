using Application.Common;

namespace Application.UseCases.Users.CreateUser;

public interface ICreateUser
{
    Result<CreateUserResult> Execute(CreateUserCommand createUserCommand);
}
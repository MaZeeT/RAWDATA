using Application.Common;
using Application.Interfaces.Repositories;

namespace Application.UseCases.Users.LoginUser;

public class LoginUser : ILoginUser
{
    private readonly IUserRepository _userRepository;

    public LoginUser(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Result<LoginUserResult> Execute(LoginUserCommand loginUserCommand)
    {
        var user = _userRepository.GetAppUser(loginUserCommand.Username);

        if (user is null) return Result<LoginUserResult>.Failure("User not found");

        return Result<LoginUserResult>.Success(new LoginUserResult
        {
            UserId = user.Id,
            UserName = user.Username
        });
    }
}
using Application.Common;
using Application.Interfaces.Repositories;

namespace Application.UseCases.Users.GetUser;

public class GetUser : IGetUser
{
    private readonly IUserRepository _userRepository;

    public GetUser(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Result<GetUserResult> Execute(GetUserCommand getUserCommand)
    {
        var username = _userRepository.GetAppUserName(getUserCommand.UserId);

        if (username is null) return Result<GetUserResult>.Failure("User not found");

        return Result<GetUserResult>.Success(new GetUserResult
        {
            Username = username
        });
    }
}
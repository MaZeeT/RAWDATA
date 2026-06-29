namespace Application.UseCases.Users.LoginUser;

public class LoginUserResult
{
    public required int UserId { get; init; }
    public required string UserName { get; init; }
}
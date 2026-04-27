namespace Application.Use_Cases.LoginUser;

public class LoginUserCommand
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
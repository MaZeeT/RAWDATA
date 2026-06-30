namespace Application.UseCases.Users.LoginUser;

public record LoginUserCommand
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
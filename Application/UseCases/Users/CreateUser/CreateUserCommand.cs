namespace Application.UseCases.Users.CreateUser;

public record CreateUserCommand
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
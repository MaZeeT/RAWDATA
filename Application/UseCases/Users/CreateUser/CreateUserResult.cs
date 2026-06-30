namespace Application.UseCases.Users.CreateUser;

public record CreateUserResult
{
    public required string Username { get; init; }
}
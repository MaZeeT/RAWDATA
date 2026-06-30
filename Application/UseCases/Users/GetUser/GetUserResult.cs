namespace Application.UseCases.Users.GetUser;

public record GetUserResult
{
    public required string Username { get; init; }
}
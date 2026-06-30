namespace Application.UseCases.Users.GetUser;

public record GetUserQuery
{
    public required int UserId { get; init; }
}
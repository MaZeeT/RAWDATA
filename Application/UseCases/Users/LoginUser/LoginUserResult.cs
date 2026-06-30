namespace Application.UseCases.Users.LoginUser;

public record LoginUserResult
{
    public required int UserId { get; init; }
    public required string UserName { get; init; }
}
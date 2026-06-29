using System.Diagnostics.CodeAnalysis;

namespace Application.UseCases.Users.CreateUser;

public class CreateUserResult
{
    public CreateUserResult()
    {
    }

    [SetsRequiredMembers]
    public CreateUserResult(string username)
    {
        Username = username;
    }

    public required string Username { get; init; }
}
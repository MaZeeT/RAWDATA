namespace Application.Use_Cases.CreateUser;

public class CreateUserCommand
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
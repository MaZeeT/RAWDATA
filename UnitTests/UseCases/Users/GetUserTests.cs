using Application.UseCases.Users.GetUser;
using Domain.Entities;
using UnitTests.Stubs.RepositoryStubs;
using Xunit;

namespace UnitTests.UseCases.Users;

public class GetUserTests
{
    private readonly UserRepositoryStub _userRepository = new();
    private readonly GetUser _sut;

    public GetUserTests()
    {
        _sut = new GetUser(_userRepository);
    }

    private static GetUserCommand ValidCommand(int userId = 1)
        => new() { UserId = userId };

    // ---- success path ----

    [Fact]
    public void Execute_ExistingUserId_ReturnsSuccess()
    {
        _userRepository.Seed(new AppUser { Id = 1, Username = "TestUsername" });

        var result = _sut.Execute(ValidCommand());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Execute_ExistingUserId_ReturnsCorrectUsername()
    {
        _userRepository.Seed(new AppUser { Id = 1, Username = "TestUsername" });

        var result = _sut.Execute(ValidCommand());

        Assert.Equal("TestUsername", result.Value?.Username);
    }

    // ---- failure path ----

    [Fact]
    public void Execute_UsernameDoesNotExist_ReturnsFailure()
    {
        var result = _sut.Execute(ValidCommand());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Execute_UsernameDoesNotExist_ReturnsUserNotFoundError()
    {
        var result = _sut.Execute(ValidCommand());

        Assert.Equal("User not found", result.Error);
    }
}
using Application.UseCases.Users.LoginUser;
using Domain.Entities;
using UnitTests.Stubs.RepositoryStubs;
using Xunit;

namespace UnitTests.UseCases.Users;

public class LoginUserTests
{
    private readonly UserRepositoryStub _userRepository = new();
    private readonly LoginUser _sut;

    public LoginUserTests()
    {
        _sut = new LoginUser(_userRepository);
    }

    private static LoginUserCommand ValidCommand(string username = "TestUsername", string password = "TestPassword")
        => new() { Username = username, Password = password };

    // ---- success path ----

    [Fact]
    public void Execute_ExistingUsername_ReturnsSuccess()
    {
        _userRepository.Seed(new AppUser { Id = 1, Username = "TestUsername" });

        var result = _sut.Execute(ValidCommand());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Execute_ExistingUsername_ReturnsCorrectUserId()
    {
        _userRepository.Seed(new AppUser { Id = 1, Username = "TestUsername" });

        var result = _sut.Execute(ValidCommand());

        Assert.Equal(1, result.Value?.UserId);
    }

    [Fact]
    public void Execute_ExistingUsername_ReturnsCorrectUsername()
    {
        _userRepository.Seed(new AppUser { Id = 1, Username = "TestUsername" });

        var result = _sut.Execute(ValidCommand());

        Assert.Equal("TestUsername", result.Value?.UserName);
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
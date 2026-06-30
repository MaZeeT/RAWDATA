using Application;
using Application.UseCases.Users.CreateUser;
using Domain.Entities;
using UnitTests.Stubs;
using UnitTests.Stubs.RepositoryStubs;
using Xunit;

namespace UnitTests.UseCases.CreateUser;

public class CreateUserTests
{
    private readonly AuthSettings _authSettings = new() { PasswordSize = 256 };
    private readonly UnitOfWorkStub _unitOfWork = new();
    private readonly UserRepositoryStub _userRepository = new();
    private readonly Application.UseCases.Users.CreateUser.CreateUser _sut;

    public CreateUserTests()
    {
        _sut = new Application.UseCases.Users.CreateUser.CreateUser(_unitOfWork, _userRepository);
    }

    private static CreateUserCommand ValidCommand(string username = "TestUsername", string password = "TestPassword")
        => new() { Username = username, Password = password };

    // ---- success path ----

    [Fact]
    public void Execute_ValidCredentials_ReturnsSuccessWithUsername()
    {
        var command = ValidCommand();
        var result = _sut.Execute(command, _authSettings);

        Assert.True(result.IsSuccess);
        Assert.Equal("TestUsername", result.Value?.Username);
    }

    [Fact]
    public void Execute_ValidCredentials_CommitsUnitOfWork()
    {
        var command = ValidCommand();
        _sut.Execute(command, _authSettings);

        Assert.True(_unitOfWork.HasBeenCalled);
        Assert.Equal((uint)1, _unitOfWork.CallCounter);
    }

    // ---- duplicate username ----

    [Fact]
    public void Execute_UsernameAlreadyExists_ReturnsFailure()
    {
        _userRepository.Seed(new AppUser { Username = "TestUsername" });

        var command = ValidCommand();
        var result = _sut.Execute(command, _authSettings);

        Assert.False(result.IsSuccess);
        Assert.Equal("Username already exists", result.Error);
    }

    [Fact]
    public void Execute_UsernameAlreadyExists_DoesNotCommitUnitOfWork()
    {
        _userRepository.Seed(new AppUser { Username = "TestUsername" });

        var command = ValidCommand();
        _sut.Execute(command, _authSettings);

        Assert.False(_unitOfWork.HasBeenCalled);
    }

    // ---- invalid credentials ----

    [Fact]
    public void Execute_UsernameTooShort_ReturnsCredentialsInvalidFailure()
    {
        var command = ValidCommand(username: "t");
        var result = _sut.Execute(command, _authSettings);

        Assert.False(result.IsSuccess);
        Assert.Equal("Credentials are not valid", result.Error);
    }

    [Fact]
    public void Execute_NullUsername_ReturnsCredentialsInvalidFailure()
    {
        var command = ValidCommand(username: null);
        var result = _sut.Execute(command, _authSettings);

        Assert.False(result.IsSuccess);
        Assert.Equal("Credentials are not valid", result.Error);
    }

    [Fact]
    public void Execute_UsernameContainsInvalidCharacters_ReturnsCredentialsInvalidFailure()
    {
        var command = ValidCommand(username: "TestUsername__");
        var result = _sut.Execute(command, _authSettings);

        Assert.False(result.IsSuccess);
        Assert.Equal("Credentials are not valid", result.Error);
    }

    [Fact]
    public void Execute_InvalidCredentials_DoesNotCommitUnitOfWork()
    {
        var command = ValidCommand(username: "t");
        _sut.Execute(command, _authSettings);

        Assert.False(_unitOfWork.HasBeenCalled);
    }
}
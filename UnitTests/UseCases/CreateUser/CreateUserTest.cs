using Application;
using Application.UseCases.Users.CreateUser;
using UnitTests.Stubs;
using UnitTests.Stubs.RepositoryStubs;
using Xunit;

namespace UnitTests.UseCases.CreateUser;

public class CreateUserTest
{
    [Fact]
    public void CreateValidUser()
    {
        // Arrange
        AuthSettings authSettings = new AuthSettings { PasswordSize = 256 };

        var unitOfWork = new UnitOfWorkStub();
        var userRepository = new UserRepositoryStub();

        var sut = new Application.UseCases.Users.CreateUser.CreateUser(unitOfWork, userRepository);
        var command = new CreateUserCommand
        {
            Username = "TestUsername",
            Password = "TestPassword"
        };

        // Act
        var result = sut.Execute(command, authSettings);

        // Assert
        Assert.True((unitOfWork.HasBeenCalled));
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("TestUsername", result.Value?.Username);
    }

    [Fact]
    public void PreventCreationOfTwoEqualUsers()
    {
        // Arrange
        AuthSettings authSettings = new AuthSettings { PasswordSize = 256 };

        var unitOfWork = new UnitOfWorkStub();
        var userRepository = new UserRepositoryStub();

        var sut = new Application.UseCases.Users.CreateUser.CreateUser(unitOfWork, userRepository);
        var command = new CreateUserCommand
        {
            Username = "TestUsername",
            Password = "TestPassword"
        };

        // Act
        var result = sut.Execute(command, authSettings);
        var result2 = sut.Execute(command, authSettings);

        // Assert
        Assert.True((unitOfWork.HasBeenCalled));
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("TestUsername", result.Value?.Username);

        Assert.Equal((uint)1, unitOfWork.CallCounter);
        Assert.NotNull(result2);
        Assert.False(result2.IsSuccess);
        Assert.Equal("Username already exists", result2.Error);
    }
    
    [Fact]
    public void FailWhenCreatingUserWithToShortUsername()
    {
        // Arrange
        AuthSettings authSettings = new AuthSettings { PasswordSize = 256 };

        var unitOfWork = new UnitOfWorkStub();
        var userRepository = new UserRepositoryStub();

        var sut = new Application.UseCases.Users.CreateUser.CreateUser(unitOfWork, userRepository);
        var command = new CreateUserCommand
        {
            Username = "t",
            Password = "TestPassword"
        };

        // Act
        var result = sut.Execute(command, authSettings);

        // Assert
        Assert.False((unitOfWork.HasBeenCalled));
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("Credentials are not valid", result.Error);
    }
    
    [Fact]
    public void FailWithNullUsername()
    {
        // Arrange
        AuthSettings authSettings = new AuthSettings { PasswordSize = 256 };

        var unitOfWork = new UnitOfWorkStub();
        var userRepository = new UserRepositoryStub();

        var sut = new Application.UseCases.Users.CreateUser.CreateUser(unitOfWork, userRepository);
        var command = new CreateUserCommand
        {
            Username = null,
            Password = "TestPassword"
        };

        // Act
        var result = sut.Execute(command, authSettings);

        // Assert
        Assert.False((unitOfWork.HasBeenCalled));
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("Credentials are not valid", result.Error);
    }
    
    [Fact]
    public void FailWithInvalidCharInUsername()
    {
        // Arrange
        AuthSettings authSettings = new AuthSettings { PasswordSize = 256 };

        var unitOfWork = new UnitOfWorkStub();
        var userRepository = new UserRepositoryStub();

        var sut = new Application.UseCases.Users.CreateUser.CreateUser(unitOfWork, userRepository);
        var command = new CreateUserCommand
        {
            Username = "TestUsernameWithFunnyChars😀",
            Password = "TestPassword"
        };

        // Act
        var result = sut.Execute(command, authSettings);

        // Assert
        Assert.False((unitOfWork.HasBeenCalled));
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("Credentials are not valid", result.Error);
    }
}
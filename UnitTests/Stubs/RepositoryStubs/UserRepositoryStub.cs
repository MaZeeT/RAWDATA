using Application.Interfaces.Repositories;
using Domain.Entities;

namespace UnitTests.Stubs.RepositoryStubs;

public class UserRepositoryStub : IUserRepository
{
    private readonly IList<AppUser> _users = new List<AppUser>();

    public void Seed(params AppUser[] users)
    {
        foreach (var u in users) _users.Add(u);
    }

    public AppUser Add(AppUser user)
    {
        _users.Add(user);
        return user;
    }

    public bool CreateAppUser(string username, string password, string salt)
    {
        throw new NotImplementedException();
    }

    public AppUser CreateUser(string name, string password, string salt)
    {
        throw new NotImplementedException();
    }

    public bool UpdateAppUserName(string oldName, string newName)
    {
        throw new NotImplementedException();
    }

    public bool DeleteAppUser(int id)
    {
        throw new NotImplementedException();
    }

    public bool DeleteAppUser(string username)
    {
        throw new NotImplementedException();
    }

    public AppUser? GetAppUser(string username)
    {
        throw new NotImplementedException();
    }

    public string? GetAppUserName(int id)
    {
        throw new NotImplementedException();
    }

    public int GetAppUserId(string username)
    {
        throw new NotImplementedException();
    }

    public bool AppUserExist(int id)
    {
        return _users.Any(x => x.Id == id);
    }

    public bool AppUserExist(string username)
    {
        return _users.Any(x => x.Username == username);
    }

    public bool AppUserExist(AppUser user)
    {
        return _users.Any(x => x.Username == user.Username);
    }
}
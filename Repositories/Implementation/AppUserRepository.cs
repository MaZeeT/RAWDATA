using Infrastructure;
using Infrastructure.Database;
using Domain;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class AppUserRepository : IUserRepository
{
    readonly DatabaseContext2 _database;

    public AppUserRepository(IDbContextFactory<DatabaseContext2> factory)
    {
        _database = factory.CreateDbContext();
    }

    public string GetAppUserName(int id)
    {
        var result = _database.AppUser.Find(id);
        return result.Username;
    }

    public int GetAppUserId(string username)
    {
        var user = GetAppUser(username);
        if (user == null)
        {
            return -1;
        }
        else
        {
            return user.Id;
        }
    }

    /// <summary>
    /// The function handlex the potential existance of many users with the same username 
    /// But in the db we handle this by having a constraint on the table that usernamens are unique
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public AppUser GetAppUser(string username)
    {
        var appUsers = _database.AppUser.Where(user => user.Username == username).ToList();
        if (appUsers.Count > 0)
        {
            return appUsers[0];
        }

        return null;
    }

    public bool CreateAppUser(string username, string password, string salt)
    {
        if (AppUserExist(username))
        {
            return false;
        }

        _database.AppUser.Add(
            new AppUser()
            {
                Username = username,
                Password = password,
                Salt = salt
            });

        var result = _database.SaveChanges();
        return result > 0;
    }

    public AppUser CreateUser(string name, string password, string salt)
    {
        if (CreateAppUser(name, password, salt))
        {
            return GetAppUser(name);
        }

        return null;
    }

    public bool UpdateAppUserName(string oldName, string newName)
    {
        if (!AppUserExist(oldName))
        {
            return false;
        }

        var appUserId = GetAppUserId(oldName);
        var appUser = _database.AppUser.Find(appUserId);
        _database.AppUser.Update(appUser);
        appUser.Username = newName;
        var result = _database.SaveChanges();
        return result > 0;
    }

    public bool DeleteAppUser(int id)
    {
        if (!AppUserExist(id))
        {
            return false;
        }

        var appUser = _database.AppUser.Find(id);
        _database.AppUser.Remove(appUser);

        var result = _database.SaveChanges();
        return result > 0;
    }

    public bool DeleteAppUser(string username)
    {
        return DeleteAppUser(GetAppUserId(username));
    }

    public bool AppUserExist(int id)
    {
        var result = _database.AppUser.Find(id);
        return result != null;
    }

    public bool AppUserExist(string username)
    {
        return AppUserExist(GetAppUserId(username));
    }
}

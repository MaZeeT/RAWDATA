using System;
using System.Linq;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories;

public class AppUserRepository : IUserRepository
{
    private readonly DatabaseContext _dbContext;

    public AppUserRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string? GetAppUserName(int id)
    {
        var result = _dbContext.AppUser.Find(id);
        return result?.Username;
    }

    public int GetAppUserId(string username)
    {
        var user = GetAppUser(username);
        if (user == null)
        {
            return -1;
        }

        return user.Id;
    }
    
    public AppUser? GetAppUser(string username)
    {
        var appUsers = _dbContext.AppUser.Where(user => user.Username == username).ToList();
        if (appUsers.Count > 0)
        {
            return appUsers[0];
        }

        return null;
    }

    public AppUser Add(AppUser user)
    {
        _dbContext.AppUser.Add(user);
        return user;
    }

    public bool CreateAppUser(string username, string password, string salt)
    {
        if (AppUserExist(username))
        {
            return false;
        }

        _dbContext.AppUser.Add(
            new AppUser
            {
                Username = username,
                Password = password,
                Salt = salt
            });

        var result = _dbContext.SaveChanges();
        return result > 0;
    }

    public AppUser CreateUser(string name, string password, string salt)
    {
        if (CreateAppUser(name, password, salt))
        {
            return GetAppUser(name);
        }

        throw new ArgumentException("User not found");
    }

    public bool UpdateAppUserName(string oldName, string newName)
    {
        if (!AppUserExist(oldName))
        {
            return false;
        }

        var appUserId = GetAppUserId(oldName);
        var appUser = _dbContext.AppUser.Find(appUserId);
        _dbContext.AppUser.Update(appUser);
        appUser.Username = newName;
        var result = _dbContext.SaveChanges();
        return result > 0;
    }

    public bool DeleteAppUser(int id)
    {
        if (!AppUserExist(id))
        {
            return false;
        }

        var appUser = _dbContext.AppUser.Find(id);
        _dbContext.AppUser.Remove(appUser);

        var result = _dbContext.SaveChanges();
        return result > 0;
    }

    public bool DeleteAppUser(string username)
    {
        return DeleteAppUser(GetAppUserId(username));
    }

    public bool AppUserExist(int id)
    {
        var result = _dbContext.AppUser.Find(id);
        return result != null;
    }

    public bool AppUserExist(string username)
    {
        return _dbContext.AppUser.Any(user => user.Username == username);
    }

    public bool AppUserExist(AppUser user)
    {
        return _dbContext.AppUser.Any(u => u.Username == user.Username);
    }
}

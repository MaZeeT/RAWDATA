using Application.Common;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IUserService
{
    string? GetUserName(int id);
    AppUser? GetAppUser(string username);
    Result<AppUser> CreateUser(string username, string password, string salt);
    bool UserExists(string username);
}

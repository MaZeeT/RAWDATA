using BusinessLogic.Interfaces;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

class UserService : IUserService
{
    readonly IUser _userService;

    public UserService(IUser userService)
    {
        _userService = userService;
    }

    public string GetUserName(int id)
    {
        return _userService.GetAppUserName(id);
    }
}

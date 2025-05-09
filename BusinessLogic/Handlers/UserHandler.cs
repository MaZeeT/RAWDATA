using BusinessLogic.Interfaces;
using Repositories.Interfaces;

namespace BusinessLogic.Handlers;

class UserHandler : IUserHandler
{
    readonly IUser _userService;

    public UserHandler(IUser userService)
    {
        _userService = userService;
    }

    public string UserName(int id)
    {
        return _userService.GetAppUserName(id);
    }
}

using BusinessLogic.Interfaces;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

class UserService : IUserService
{
    readonly IUserRepository _userRepositoryService;

    public UserService(IUserRepository userRepositoryService)
    {
        _userRepositoryService = userRepositoryService;
    }

    public string GetUserName(int id)
    {
        return _userRepositoryService.GetAppUserName(id);
    }
}

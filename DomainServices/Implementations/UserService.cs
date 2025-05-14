using BusinessLogic.Interfaces;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

public class UserService : IUserService
{
    readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string GetUserName(int id)
    {
        return _userRepository.GetAppUserName(id);
    }
}

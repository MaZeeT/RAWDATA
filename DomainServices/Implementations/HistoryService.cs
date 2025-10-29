using Domain.Models;
using Domain.Services;
using DomainServices.Interfaces;
using Repositories.Interfaces;

namespace DomainServices.Implementations;

public class HistoryService : IHistoryService
{
    private readonly IHistoryRepository _historyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISharedRepository _sharedRepository;

    public HistoryService(IHistoryRepository historyRepository, IUserRepository userRepository, ISharedRepository sharedRepository)
    {
        _historyRepository = historyRepository;
        _userRepository = userRepository;
        _sharedRepository = sharedRepository;
    }

    public bool Add(History history)
    {
        var userExist = _userRepository.AppUserExist(history.UserId);
        if (!userExist)
        {
            return false;
        }
        
        var postType = _sharedRepository.GetPostType(history.PostId);
        if (postType == "unknown")
        {
            return false;
        }
        
        return _historyRepository.Add(history);
    }

    public List<History> GetHistoryList(int userId, PagingAttributes pagingAttributes)
    {
        return _historyRepository.GetHistoryList(userId, pagingAttributes);
    }

    public int GetCount(int userId)
    {
        return _historyRepository.GetCount(userId, false);
    }

    public bool DeleteUserHistory(int userId)
    {
        return _historyRepository.DeleteUserHistory(userId);
    }
}
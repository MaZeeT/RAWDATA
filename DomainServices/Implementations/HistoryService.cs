using BusinessLogic.Interfaces;
using Domain.Entities;
using Domain.Services;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

public class HistoryService : IHistoryService
{
    private readonly IHistoryRepository _historyRepository;

    public HistoryService(IHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public bool Add(History history)
    {
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
using BusinessLogic.Interfaces;
using Domain.Entities;
using Domain.Services;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

public class HistoryService : IHistoryService
{
    private readonly IHistoryRepository _historyRepositoryService;

    public HistoryService(IHistoryRepository historyRepositoryService)
    {
        _historyRepositoryService = historyRepositoryService;
    }

    public List<History> GetHistoryList(int userId, PagingAttributes pagingAttributes)
    {
        return _historyRepositoryService.GetHistoryList(userId, pagingAttributes);
    }

    public int GetCount(int userId)
    {
        return _historyRepositoryService.GetCount(userId, false);
    }

    public bool DeleteUserHistory(int userId)
    {
        return _historyRepositoryService.DeleteUserHistory(userId);
    }
}
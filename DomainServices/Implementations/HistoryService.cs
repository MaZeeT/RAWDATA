using Domain.Models;
using Domain.Services;
using DomainServices.Interfaces;
using Repositories.Interfaces;

namespace DomainServices.Implementations;

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
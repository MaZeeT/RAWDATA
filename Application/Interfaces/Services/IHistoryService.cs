using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IHistoryService
{
    bool Add(History history);
    List<History> GetHistoryList(int userId, PagingAttributes pagingAttributes);
    int GetCount(int userId);
    bool DeleteUserHistory(int userId);
}
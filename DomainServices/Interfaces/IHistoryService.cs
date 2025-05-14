using Domain.Entities;
using Domain.Services;

namespace BusinessLogic.Interfaces;

public interface IHistoryService
{
    List<History> GetHistoryList(int userId, PagingAttributes pagingAttributes);
    int GetCount(int userId);
    bool DeleteUserHistory(int userId);
}
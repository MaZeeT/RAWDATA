using Domain.Models;
using Domain.Services;

namespace Repositories.Interfaces;

public interface ISearchHistoryRepository
{
    bool DeleteUserSearchHistory(int userId);
    (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
}

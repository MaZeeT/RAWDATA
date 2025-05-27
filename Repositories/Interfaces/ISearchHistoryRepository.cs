using Domain;
using Domain.Models;
using Domain.Services;

namespace Repositories.Interfaces;

public interface ISearchHistoryRepository
{
    bool DeleteSearchHistory(int searchId);
    bool DeleteUserSearchHistory(int userId);
    (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
    bool SearchExist(int searchId);
}

using Domain.Models;

namespace Application.Interfaces;

public interface ISearchHistoryRepository
{
    bool DeleteUserSearchHistory(int userId);
    (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
}

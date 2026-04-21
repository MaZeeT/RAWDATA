using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISearchHistoryRepository
{
    bool DeleteUserSearchHistory(int userId);
    (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
}

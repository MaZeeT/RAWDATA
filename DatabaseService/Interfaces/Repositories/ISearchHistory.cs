using System.Collections.Generic;

namespace DatabaseService.Interfaces.Repositories;

public interface ISearchHistory
{
    bool DeleteSearchHistory(int searchId);
    bool DeleteUserSearchHistory(int userId);
    (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
    bool SearchExist(int searchId);
}

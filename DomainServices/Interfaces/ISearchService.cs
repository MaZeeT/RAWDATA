using Domain.Enums;
using Domain.Models;

namespace DomainServices.Interfaces;

public interface ISearchService
{
    IList<WordRank> WordRank(int userid, string searchString, SearchType searchType, int? maxResults);
    IList<Posts> Search(int userid, string searchString, SearchType searchType, PagingAttributes pagingAttributes);
    string BuildSearchString(string searchString, bool reverse);
    bool DeleteUserSearchHistory(int userId);
    (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
}
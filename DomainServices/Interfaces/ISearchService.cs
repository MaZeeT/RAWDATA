using Domain.Models;
using Domain.Services;

namespace DomainServices.Interfaces;

public interface ISearchService
{
    IList<WordRank> WordRank(int userid, string searchString, int searchTypeCode, int? maxResults);
    IList<Posts> Search(int userid, string searchString, int? searchTypeCode, PagingAttributes pagingAttributes);
    string BuildSearchString(string searchString, bool reverse);
    int SearchTypeLookup(string searchType);
    bool DeleteUserSearchHistory(int userId);
    (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
}
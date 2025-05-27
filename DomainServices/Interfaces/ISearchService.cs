using Domain.Models;
using Domain.Services;

namespace DomainServices.Interfaces;

public interface ISearchService
{
    IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults);
    IList<Posts> Search(int userid, string searchstring, int? searchtypecode, PagingAttributes pagingAttributes);
    string BuildSearchString(string searchstring, bool reverse);
    int SearchTypeLookup(string searchType);
    bool DeleteUserSearchHistory(int userId);
    (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
}
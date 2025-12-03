using Domain.Models;
using Domain.Services;

namespace Repositories.Interfaces;

public interface ISearchRepository
{
    IList<WordRank> WordRank(int userid, string searchString, int searchTypeCode, int? maxResults);
    IList<Posts> Search(int userid, string searchString, int? searchTypeCode, PagingAttributes pagingAttributes);
    string BuildSearchString(string searchString, bool reverse);
    int SearchTypeLookup(string searchType);
}

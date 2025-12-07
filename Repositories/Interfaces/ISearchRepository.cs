using Domain.Enums;
using Domain.Models;
using Domain.Services;

namespace Repositories.Interfaces;

public interface ISearchRepository
{
    IList<WordRank> WordRank(int userid, string searchString, SearchType searchType, int? maxResults);
    IList<Posts> Search(int userid, string searchString, SearchType searchType, PagingAttributes pagingAttributes);
    string BuildSearchString(string searchString, bool reverse);
}

using Domain;
using Domain.Models;
using Domain.Services;

namespace Repositories.Interfaces;

public interface ISearchRepository
{
    IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults);
    IList<Posts> Search(int userid, string searchstring, int? searchtypecode, PagingAttributes pagingAttributes);
    string BuildSearchString(string searchstring, bool reverse);
    int SearchTypeLookup(string stype);
}

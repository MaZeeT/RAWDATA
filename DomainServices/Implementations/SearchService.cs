using BusinessLogic.Interfaces;
using Domain.Entities;
using Domain.Services;

namespace BusinessLogic.Implementations;

public class SearchService : ISearchService
{
    private readonly ISearchService _searchService;

    public SearchService(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults)
    {
        return _searchService.WordRank(userid, searchstring, searchtypecode, maxresults);
    }

    public IList<Posts> Search(int userid, string searchstring, int? searchtypecode, PagingAttributes pagingAttributes)
    {
        return _searchService.Search(userid, searchstring, searchtypecode, pagingAttributes);
    }

    public string BuildSearchString(string searchstring, bool reverse)
    {
        return _searchService.BuildSearchString(searchstring, reverse);
    }
}
using BusinessLogic.Interfaces;
using Domain.Entities;
using Domain.Services;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepository;
    private readonly ISearchHistoryRepository _searchHistoryRepository;

    public SearchService(ISearchRepository searchRepository, ISearchHistoryRepository searchHistoryRepository)
    {
        _searchRepository = searchRepository;
        _searchHistoryRepository = searchHistoryRepository;
    }

    public IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults)
    {
        return _searchRepository.WordRank(userid, searchstring, searchtypecode, maxresults);
    }

    public IList<Posts> Search(int userid, string searchstring, int? searchtypecode, PagingAttributes pagingAttributes)
    {
        return _searchRepository.Search(userid, searchstring, searchtypecode, pagingAttributes);
    }

    public string BuildSearchString(string searchstring, bool reverse)
    {
        return _searchRepository.BuildSearchString(searchstring, reverse);
    }

    public int SearchTypeLookup(string searchType)
    {
        return _searchRepository.SearchTypeLookup(searchType);
    }

    public bool DeleteUserSearchHistory(int userId)
    {
        return _searchHistoryRepository.DeleteUserSearchHistory(userId);
    }

    public (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes)
    {
        return _searchHistoryRepository.GetSearchesList(userId, pagingAttributes);
    }
}
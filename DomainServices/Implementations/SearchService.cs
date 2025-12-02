using Domain.Models;
using Domain.Services;
using DomainServices.Interfaces;
using Repositories.Interfaces;

namespace DomainServices.Implementations;

public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepository;
    private readonly ISearchHistoryRepository _searchHistoryRepository;
    private readonly IUserRepository _userRepository;

    public SearchService(ISearchRepository searchRepository, ISearchHistoryRepository searchHistoryRepository, IUserRepository userRepository)
    {
        _searchRepository = searchRepository;
        _searchHistoryRepository = searchHistoryRepository;
        _userRepository = userRepository;
    }

    public IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults)
    {
        var userExist = _userRepository.AppUserExist(userid);
        if (!userExist)
        {
            return new List<WordRank>();
        }
        
        return _searchRepository.WordRank(userid, searchstring, searchtypecode, maxresults);
    }

    public IList<Posts> Search(int userid, string searchstring, int? searchtypecode, PagingAttributes pagingAttributes)
    {
        var userExist = _userRepository.AppUserExist(userid);
        if (!userExist)
        {
            return new List<Posts>();
        }
        
        
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
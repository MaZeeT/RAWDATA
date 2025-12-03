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

    public IList<WordRank> WordRank(int userid, string searchString, int searchTypeCode, int? maxResults)
    {
        var userExist = _userRepository.AppUserExist(userid);
        if (!userExist)
        {
            return new List<WordRank>();
        }
        
        return _searchRepository.WordRank(userid, searchString, searchTypeCode, maxResults);
    }

    public IList<Posts> Search(int userid, string searchString, int? searchTypeCode, PagingAttributes pagingAttributes)
    {
        var userExist = _userRepository.AppUserExist(userid);
        if (!userExist)
        {
            return new List<Posts>();
        }
        
        
        return _searchRepository.Search(userid, searchString, searchTypeCode, pagingAttributes);
    }

    public string BuildSearchString(string searchString, bool reverse)
    {
        return _searchRepository.BuildSearchString(searchString, reverse);
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
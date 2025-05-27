using Domain.Models;
using Domain.Services;
using DomainServices.Interfaces;
using Repositories.Interfaces;

namespace DomainServices.Implementations;

public class BookmarkService : IBookmarkService
{
    private readonly IHistoryRepository _historyRepository;

    public BookmarkService(IHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public bool Add(int userId, int postId)
    {
        return _historyRepository.Add(userId, postId, true);
    }

    public bool DeleteBookmark(int userId, int postId)
    {
        return _historyRepository.DeleteBookmark(userId, postId);
    }

    public List<History> GetBookmarkList(int userId)
    {
        return _historyRepository.GetBookmarkList(userId);
    }

    public List<History> GetBookmarkList(int userId, PagingAttributes pagingAttributes)
    {
        return _historyRepository.GetBookmarkList(userId, pagingAttributes);
    }

    public int GetCount(int userId)
    {
        return _historyRepository.GetCount(userId, true);
    }
}
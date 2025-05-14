using BusinessLogic.Interfaces;
using Domain.Entities;
using Domain.Services;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

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
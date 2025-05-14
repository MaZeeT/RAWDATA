using BusinessLogic.Interfaces;
using Domain.Entities;
using Domain.Services;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

public class BookmarkService : IBookmarkService
{
    private readonly IHistoryRepository _historyRepositoryService;

    public BookmarkService(IHistoryRepository historyRepositoryService)
    {
        _historyRepositoryService = historyRepositoryService;
    }

    public bool Add(int userId, int postId)
    {
        return _historyRepositoryService.Add(userId, postId, true);
    }

    public bool DeleteBookmark(int userId, int postId)
    {
        return _historyRepositoryService.DeleteBookmark(userId, postId);
    }

    public List<History> GetBookmarkList(int userId)
    {
        return _historyRepositoryService.GetBookmarkList(userId);
    }

    public List<History> GetBookmarkList(int userId, PagingAttributes pagingAttributes)
    {
        return _historyRepositoryService.GetBookmarkList(userId, pagingAttributes);
    }

    public int GetCount(int userId)
    {
        return _historyRepositoryService.GetCount(userId, true);
    }
}
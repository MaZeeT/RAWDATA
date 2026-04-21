using Application.Interfaces;
using Domain.Entities;
using DomainServices.Interfaces;

namespace DomainServices.Implementations;

public class BookmarkService : IBookmarkService
{
    private readonly IHistoryRepository _historyRepository;
    private readonly ISharedRepository _sharedRepository;

    public BookmarkService(IHistoryRepository historyRepository, ISharedRepository sharedRepository)
    {
        _historyRepository = historyRepository;
        _sharedRepository = sharedRepository;
    }

    public bool Add(int userId, int postId)
    {
        var postType = _sharedRepository.GetPostType(postId);

        var history = new History
        {
            UserId = userId,
            PostId = postId,
            PostTableName = postType,
            Date = DateTime.Now,
            IsBookmark = true
        };
        
        return _historyRepository.Add(history);
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
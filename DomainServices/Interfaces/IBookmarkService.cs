using Domain.Models;

namespace DomainServices.Interfaces;

public interface IBookmarkService
{
    bool Add(int userId, int postId);
    bool DeleteBookmark(int userId, int postId);
    List<History> GetBookmarkList(int userId);
    List<History> GetBookmarkList(int userId, PagingAttributes pagingAttributes);
    int GetCount(int userId);
}
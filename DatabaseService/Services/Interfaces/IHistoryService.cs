using DatabaseService.Modules;
using System.Collections.Generic;

namespace DatabaseService.Services
{
    public interface IHistoryService
    {
        bool Add(int UserId, int PostId, bool isBookmark);
        bool Add(History history);
        History Get(int historyId);
        History Get(int userId, int postId);
        List<History> GetHistoryList(int userId);
        List<History> GetHistoryList(int userId, PagingAttributes pagingAttributes);
        List<History> GetBookmarkList(int userId);
        List<History> GetBookmarkList(int userId, PagingAttributes pagingAttributes);
        bool DeleteUserHistory(int userId);
        bool DeleteHistory(int historyId);
        bool DeleteBookmark(int userId, int postId);
        bool HistoryExist(int historyId);
        bool HistoryExist(int userId, int postId);
    }
}

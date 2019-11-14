using DatabaseService.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseService.Services
{
    public interface IHistoryService
    {
        bool Add(int UserId, int PostId, bool isBookmark);
        bool Add(History history);
        History Get(int historyId);
        History Get(int userId, int postId);
        List<History> GetHistoryList(int userId);
        List<History> GetBookmarkList(int userId);
        bool Delete(int historyId);
        bool DeleteBookmark(int userId, int postId);
        bool HistoryExist(int historyId);
        bool HistoryExist(int userId, int postId);
    }
}

using DatabaseService.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseService.Services
{
    public interface IHistoryService
    {
        bool Add(History history);
        bool Add(int UserId, int PostId, bool isBookmark);
        History Get(int historyId);
        History Get(int userId, int postId);
        bool Delete(int historyId);
        bool HistoryExist(int historyId);
        bool HistoryExist(int userId, int postId);
    }
}

using System.Collections.Generic;
using System.Linq;
using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DatabaseService.Services
{
    public class HistoryService : IHistoryService
    {
        DatabaseContext database;

        public HistoryService()
        {
            database = new DatabaseContext();
        }
        
        
        public bool Add(int UserId, int PostId, bool isBookmark)
        {
            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            var ipostid = new NpgsqlParameter("ipostid", NpgsqlTypes.NpgsqlDbType.Integer);
            var addbookmark = new NpgsqlParameter("addbookmark", NpgsqlTypes.NpgsqlDbType.Boolean);

            appuserid.Value = UserId;
            ipostid.Value = PostId;
            addbookmark.Value = isBookmark;

            database.Database.ExecuteSqlRaw(
                "SELECT * from add_history(@appuserid, @ipostid, @addbookmark)",
                appuserid, ipostid, addbookmark);

            return HistoryExist(UserId, PostId);
        }

        public bool Add(History history)
        {
            return Add(history.Userid, history.Postid, history.isBookmark);
        }

        public History Get(int historyId)
        {
            return database.History.Find(historyId);
        }

        public History Get(int userId, int postId)
        {
            var histories = database.History.Where(user => user.Userid == userId && user.Postid == postId).ToList();
            if (histories.Count > 0)
            {
                return histories.First();
            }
            else
            {
                return null;
            }
        }

        public List<History> GetHistoryList(int userId)
        {
            PagingAttributes pagingAttributes = new PagingAttributes();
            return GetHistoryList(userId, pagingAttributes);
        }

        public List<History> GetHistoryList(int userId, PagingAttributes pageAtt)
        {
            return GetListFromQuery(userId, false, pageAtt);
        }

        public List<History> GetBookmarkList(int userId)
        {
            PagingAttributes pagingAttributes = new PagingAttributes();
            return GetBookmarkList(userId, pagingAttributes);
        }

        public List<History> GetBookmarkList(int userId, PagingAttributes pageAtt)
        {
            return GetListFromQuery(userId, true, pageAtt);
        }

        public bool DeleteUserHistory(int userId)
        {
            var history = database.History.Where(x =>
                x.Userid == userId &&
                x.isBookmark == false);

            foreach (var entry in history)
            {
                database.History.Remove(entry);
            }

            return database.SaveChanges() > 0;
        }

        public bool DeleteHistory(int historyId)
        {
            if (HistoryExist(historyId))
            {
                var history = database.History.Find(historyId);
                database.History.Remove(history);

                return database.SaveChanges() > 0;
            }

            return false;
        }

        public bool DeleteBookmark(int userId, int postId)
        {
            var history = database.History.Where(x =>
                x.Userid == userId &&
                x.Postid == postId &&
                x.isBookmark == true);

            foreach (var h in history)
            {
                database.History.Update(h);
                h.isBookmark = false;
            }

            return database.SaveChanges() > 0;
        }

        public bool HistoryExist(int historyId)
        {
            var result = database.History.Find(historyId);
            return result != null;
        }

        public bool HistoryExist(int userId, int postId)
        {
            var result = database.History.Where(history =>
                    history.Userid == userId &&
                    history.Postid == postId)
                .ToList();

            return result.Count > 0;
        }

        private List<History> GetListFromQuery(int userId, bool isBookmark, PagingAttributes pageAtt)
        {
            // This enforces the page upper and lower limits
            SharedService sharedService = new SharedService();
            sharedService.GetPagination(GetCount(userId, true), pageAtt);

            return database.History
                .Where(x =>
                    x.Userid == userId &&
                    x.isBookmark == isBookmark)
                .OrderBy(x => x.Date)
                .Skip((pageAtt.Page - 1) * pageAtt.PageSize)
                .Take(pageAtt.PageSize)
                .ToList();
        }

        private int GetCount(int userId, bool isBookmark)
        {
            return database.History.Count(x =>
                x.Userid == userId &&
                x.isBookmark == isBookmark);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DatabaseService.Services
{
    public class HistoryService : IHistoryService
    {
        AppContext database;

        public HistoryService()
        {
            database = new AppContext();
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
            var list = database.History
                .Where(x => x.Userid == userId)
                .OrderBy(x => x.Date)
                .ToList();
                
            return list;
        }
        
        public List<History> GetBookmarks(int userId)
        {
            var list = database.History
                .Where(x => x.Userid == userId && x.isBookmark == true)
                .OrderBy(x => x.Date)
                .ToList();
                
            return list;
        }
        

        public bool Delete(int historyId)
        {
            if (HistoryExist(historyId))
            {
                var history = database.History.Find(historyId);
                database.History.Remove(history);

                var result = database.SaveChanges();
                return result > 0;
            }

            return false;
        }

        public bool HistoryExist(int historyId)
        {
            var result = database.History.Find(historyId);
            return result != null;
        }

        public bool HistoryExist(int userId, int postId)
        {
            var result = database.History.Where(history => history.Userid == userId && history.Postid == postId)
                .ToList();
            return result.Count > 0;
        }
    }
}
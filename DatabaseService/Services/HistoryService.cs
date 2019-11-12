using System;
using System.Data.SqlClient;
using System.Linq;
using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;

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
            //string query = "select add_history(@appuserid, @ipostid, @addbookmark);";
            // database.History.FromSqlRaw(query, history.Userid, history.Postid, history.isBookmark);

            /*  
              //string query = "EXECUTE add_history({Userid}, {Postid}, {isBookmark};";
              string query = "EXECUTE add_history(0, 709, false;";
              database.History.FromSqlRaw(query);
              var parameters = new SqlParameter("appuserid", history.Userid);
  
              appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search
  */
            var user = new SqlParameter("appuserid", UserId);
            var post = new SqlParameter("postid", PostId);
            var mark = new SqlParameter("isbookmark", isBookmark);

            //string query = "add_history @appuserid, @postid, @isbookmark";
            //database.History.FromSqlRaw(query, user, post, mark);
            database.History.FromSqlRaw("exec add_history({0},{1},{2})", user, post, mark);

            throw new System.NotImplementedException();
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
            var result = database.History.Where(history => history.Userid == userId && history.Postid == postId).ToList();
            return result.Count > 0;
        }
        
    }
}
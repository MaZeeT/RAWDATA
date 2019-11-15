using System.Collections.Generic;
using System.Linq;
using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DatabaseService
{
    public class SearchHistoryService : ISearchHistoryService
    {
        private readonly ISharedService _sharedService; //shared stuff by injection
        public SearchHistoryService(
            ISharedService sharedService)
        {
            _sharedService = sharedService;
        }


        /*   public History Get(int historyId)
           {
               return db.History.Find(historyId);
           }

           public History Get(int userId, int postId)
           {
               var histories = db.History.Where(user => user.Userid == userId && user.Postid == postId).ToList();
               if (histories.Count > 0)
               {
                   return histories.First();
               }
               else
               {
                   return null;
               }
           }
           */
        public (List<Searches>,int) GetSearchesList(int userId, PagingAttributes pagingAttributes)
        {
            using var db = new DatabaseContext();

            //try to convert back from 1-based pages
            int page;
            if (pagingAttributes.Page <= 0)
            {
                page = 0;
            }
            else page = pagingAttributes.Page - 1;

            var list = db.Searches
                .Where(x =>
                    x.UserId == userId)
                .OrderBy(x => x.Date)
                .Skip(page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();

            var count = db.Searches
    .Where(x =>
        x.UserId == userId)
    .OrderBy(x => x.Date)
    .Count();



            return (list,count);
        }
        /*
                public List<History> GetBookmarkList(int userId)
                {
                    var list = db.History
                        .Where(x => 
                            x.Userid == userId && 
                            x.isBookmark == true)
                        .OrderBy(x => x.Date)
                        .ToList();

                    return list;
                }
                */
        public bool DeleteUserSearchHistory(int userId)
        {
            using var db = new DatabaseContext();
            var history = db.Searches.Where(x =>
                x.UserId == userId);

            foreach (var entry in history)
            {
                db.Searches.Remove(entry);
            }

            return db.SaveChanges() > 0;
        }

        public bool DeleteSearchHistory(int searchId)
        {
            using var db = new DatabaseContext();
            if (SearchExist(searchId))
            {
                var history = db.History.Find(searchId);
                db.History.Remove(history);

                return db.SaveChanges() > 0;
            }

            return false;
        }
        /*
                public bool DeleteBookmark(int userId, int postId)
                {
                    var history = db.History.Where(x =>
                        x.Userid == userId &&
                        x.Postid == postId &&
                        x.isBookmark == true);

                    foreach (var h in history)
                    {
                        db.History.Update(h);
                        h.isBookmark = false;
                    }

                    return db.SaveChanges() > 0;
                }
                */
        public bool SearchExist(int searchId)
        {
            using var db = new DatabaseContext();
            var result = db.Searches.Find(searchId);
            return result != null;
        }
        /*
        public bool SearchExist(int userId, int postId)
        {
            var result = db.History.Where(history =>
                    history.Userid == userId &&
                    history.Postid == postId)
                .ToList();
            return result.Count > 0;
        }*/
    }
}
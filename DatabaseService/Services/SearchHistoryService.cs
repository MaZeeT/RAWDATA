using System.Collections.Generic;
using System.Linq;

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

        public (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes)
        {
            using var db = new DatabaseContext();

            var count = db.Searches
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Date)
                .Count();

            //try to convert back from 1-based pages
            int page = _sharedService.GetPagination(count, pagingAttributes);

            var list = db.Searches
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Date)
                .Skip(page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();

            return (list, count);
        }

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

        public bool SearchExist(int searchId)
        {
            using var db = new DatabaseContext();
            var result = db.Searches.Find(searchId);
            return result != null;
        }
    }
}
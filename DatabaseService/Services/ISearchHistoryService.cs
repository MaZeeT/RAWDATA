using DatabaseService.Modules;
using System.Collections.Generic;

namespace DatabaseService
{ 
    public interface ISearchHistoryService
    {
        bool DeleteSearchHistory(int searchId);
        bool DeleteUserSearchHistory(int userId);
        public (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes);
        bool SearchExist(int searchId);
    }
}

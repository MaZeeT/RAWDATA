using System.Collections.Generic;


namespace DatabaseService
{
    public interface IDataService
    {
        IList<Questions> BrowseQuestions(PagingAttributes pagingAttributes);
        IList<Search> Search(string searchstring, int? searchtypecode);
        IList<WordRank> WordRank(string searchstring, int? searchtypecode);
    }
}

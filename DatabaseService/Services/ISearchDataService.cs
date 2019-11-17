using System.Collections.Generic;


namespace DatabaseService
{
    public interface ISearchDataService
    {
        IList<Questions> GetQuestions(PagingAttributes pagingAttributes);
        IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults);
        IList<Posts> Search(int userid, string searchstring, int? searchtypecode, PagingAttributes pagingAttributes);
        string BuildSearchString(string searchstring, bool reverse);
        int SearchTypeLookup(string stype);
    }
}

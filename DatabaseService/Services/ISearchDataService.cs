using System.Collections.Generic;


namespace DatabaseService
{
    public interface ISearchDataService
    {
        IList<Questions> GetQuestions(PagingAttributes pagingAttributes);
      //  int NumberOfQuestions();
        //Questions GetQuestion(int questionId);
        IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults);
      //  IList<Posts> GetThread(int questionId);
       // string GetPostType(int postId);
        IList<Posts> Search(int userid, string searchstring, int? searchtypecode, PagingAttributes pagingAttributes);
        string BuildSearchString(string searchstring, bool reverse);
        int SearchTypeLookup(string stype);
        //  SinglePost GetPost(int postId);
    }
}

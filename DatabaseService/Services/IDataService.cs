using System.Collections.Generic;


namespace DatabaseService
{
    public interface IDataService
    {
        IList<Questions> GetQuestions(PagingAttributes pagingAttributes);
        int NumberOfQuestions();
        Questions GetQuestion(int questionId);
        IList<Search> Search(string searchstring, int? searchtypecode, PagingAttributes pagingAttributes);
        IList<WordRank> WordRank(string searchstring, int? searchtypecode, PagingAttributes pagingAttributes);
        //(Questions, IList<Answers>) GetThread(int questionId);
        IList<Posts> GetThread(int questionId);
        void GetPost(int postId);
    }
}

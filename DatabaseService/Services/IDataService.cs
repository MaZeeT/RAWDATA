using System.Collections.Generic;


namespace DatabaseService
{
    public interface IDataService
    {
        IList<Questions> Getquestions(PagingAttributes pagingAttributes);
        IList<Search> Search(string searchstring);
    }
}

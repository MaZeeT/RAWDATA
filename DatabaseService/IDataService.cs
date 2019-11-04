using System.Collections.Generic;


namespace DatabaseService
{
    public interface IDataService
    {
        IList<Questions> Getquestions(PagingAttributes pagingAttributes);

        int NumberOfCategories();

        Questions GetCategory(int categoryId);

        void CreateCategory(Questions category);

        void UpdateCategory(Questions category);

        bool DeleteCategory(int categoryId);

        bool CategoryExcist(int categoryId);
        IList<Search> Search(string searchstring);
    }
}

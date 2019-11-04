using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace DatabaseService
{
    public class DataService : IDataService
    {
        public bool CategoryExcist(int categoryId)
        {
            return GetCategory(categoryId) != null;         
        }

        public void CreateCategory(Questions category)
        {
            using var db = new StackoverflowContext();
            category.Id = db.Questions.Max(x => x.Id) + 1;
            db.Questions.Add(category);
            db.SaveChanges();
        }

        public bool DeleteCategory(int categoryId)
        {
            using var db = new StackoverflowContext();
            var category = db.Questions.Find(categoryId);
            if (category == null) return false;
            db.Questions.Remove(category);
            return db.SaveChanges() > 0;
        }

        public IList<Questions> Getquestions(PagingAttributes pagingAttributes)
        {
            using var db = new StackoverflowContext();
            return db.Questions
                .Skip(pagingAttributes.Page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();
        }

        public IList<Search> Search(string searchstring)
        {
            using var db = new StackoverflowContext();
            var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text);
            search.Value = searchstring;
            var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
            searchtype.Value = "bestmatch";
            return db.Search
                .FromSqlRaw("SELECT * from appsearch(2, @searchtype, @search) limit 10", searchtype, search)
                .ToList();
        }

        public int NumberOfCategories()
        {
            using var db = new StackoverflowContext();
            return db.Questions.Count();
        }

        public Questions GetCategory(int categoryId)
        {
            using var db = new StackoverflowContext();
            return db.Questions.Find(categoryId);
        }

        public void UpdateCategory(Questions category)
        {
            using var db = new StackoverflowContext();
            db.Questions.Update(category);
            db.SaveChanges();
        }


    }
}

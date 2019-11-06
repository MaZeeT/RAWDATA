using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace DatabaseService
{
    public class DataService : IDataService
    {

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
            var result = db.Search
                .FromSqlRaw("SELECT * from appsearch(2, @searchtype, @search) limit 10", searchtype, search)
                .ToList();
            return result;
        }

    }
}

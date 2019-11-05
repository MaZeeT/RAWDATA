using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace DatabaseService
{
    public class DataService : IDataService
    {

        public IList<Questions> BrowseQuestions(PagingAttributes pagingAttributes)
        {
            // AuthUser()
            // if ok do browse q-list
            using var db = new StackoverflowContext();
            return db.Questions
                .Skip(pagingAttributes.Page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();
        }

        public IList<Search> Search(string searchstring, int? searchtypecode)
        {

            // AuthUser()
            // if ok AddSearchHistory()
            // do actual search using appsearch in db and build results
            using var db = new StackoverflowContext();
            Modules.SearchTypeLookupTable st = new Modules.SearchTypeLookupTable();
            var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text);
            search.Value = searchstring;
            //search.Value = "chocolate fudge";

            var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
            if (searchtypecode >= 0 && searchtypecode <= 3)
            {
                searchtype.Value = st.searchType[searchtypecode.Value];
            }
            else searchtype.Value = st.searchType[3];

            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            appuserid.Value = 2;

            return db.Search
                .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search) limit 10", appuserid, searchtype, search)
                .ToList();
        }

        public IList<WordRank> WordRank(string searchstring, int? searchtypecode)
        {

            // AuthUser()
            // if ok AddSearchHistory()
            // do actual search using appsearch in db and build results
            using var db = new StackoverflowContext();
            Modules.SearchTypeLookupTable st = new Modules.SearchTypeLookupTable();

            var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text);
            search.Value = searchstring;
            //search.Value = "chocolate fudge";

            var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
            if (searchtypecode >= 4 && searchtypecode <= 5)
            {
                searchtype.Value = st.searchType[searchtypecode.Value];
            }
            else searchtype.Value = st.searchType[5];

            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            appuserid.Value = 2;

            return db.WordRank
                .FromSqlRaw("SELECT * from wordrank(@appuserid, @searchtype, @search) limit 10", appuserid, searchtype, search)
                .ToList();
        }

    }
}

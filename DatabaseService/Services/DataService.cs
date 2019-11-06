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
            ////// for performing searches with appsearch on the db
            ///
            // AuthUser()
            // if ok AddSearchHistory()
            // do actual search using appsearch in db and build results

            //need db context and searchtype lookuptable
            using var db = new StackoverflowContext();
            Modules.SearchTypeLookupTable st = new Modules.SearchTypeLookupTable();

            ////get params for db.func
            ///
            //build searchstring
            var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text);
            search.Value = BuildSearchString(searchstring);

            //lookup searchtype string
            var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
            if (searchtypecode >= 0 && searchtypecode <= 3)
            {
                searchtype.Value = st.searchType[searchtypecode.Value];
            }
            else searchtype.Value = st.searchType[3];

            //userid is hardcoded for now; should be returned from auth or sth
            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            appuserid.Value = 2;

            //call db.func appsearch
            return db.Search
                .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search) limit 10", appuserid, searchtype, search)
                .ToList();
        }

        private static string BuildSearchString(string searchstring)
        {
            string[] separators = { ",", ".", "..." };

            string[] words = searchstring.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            System.Console.WriteLine($"{words.Length} tokens in search");

            string finalstring = string.Join(" ", words);
            System.Console.WriteLine("Built search string: " + finalstring);
            return finalstring;
        }

        public IList<WordRank> WordRank(string searchstring, int? searchtypecode)
        {
            ////// for performing searches with wordrank on the db
            ///
            // AuthUser()
            // if ok AddSearchHistory()
            // do actual search using appsearch in db and build results

            //need db context and searchtype lookuptable
            using var db = new StackoverflowContext();
            Modules.SearchTypeLookupTable st = new Modules.SearchTypeLookupTable();

            ////get params for db.func
            ///
            //build searchstring
            var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text);
            search.Value = BuildSearchString(searchstring);

            //lookup searchtype string
            var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
            if (searchtypecode >= 4 && searchtypecode <= 5)
            {
                searchtype.Value = st.searchType[searchtypecode.Value];
            }
            else searchtype.Value = st.searchType[5];

            //userid is hardcoded for now; should be returned from auth or sth
            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            appuserid.Value = 2;

            //call db.func wordrank
            return db.WordRank
                .FromSqlRaw("SELECT * from wordrank(@appuserid, @searchtype, @search) limit 10", appuserid, searchtype, search)
                .ToList();
        }

    }
}

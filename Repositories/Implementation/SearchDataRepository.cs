using Domain.Services;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class SearchDataRepository : ISearchRepository
{
    private readonly IDbContextFactory<DatabaseContext2> _dbContextFactory;
    private readonly IQuestionRepository _questionRepository;
    private readonly ISharedRepository _sharedRepositoryService; //shared stuff by injection

    public SearchDataRepository(IDbContextFactory<DatabaseContext2> factory, IQuestionRepository questionRepository,
        ISharedRepository sharedRepositoryService)
    {
        _dbContextFactory = factory;
        _questionRepository = questionRepository;
        _sharedRepositoryService = sharedRepositoryService;
    }

    public IList<Posts> Search(int userid, string searchstring, int? searchtypecode,
        PagingAttributes pagingAttributes)
    {
        ////// for performing searches with appsearch on the db
        ///
        // do actual search using appsearch in db and build results

        //need db context and searchtype lookuptable
        using var db = _dbContextFactory.CreateDbContext();
        SearchTypeLookupTable st = new SearchTypeLookupTable();

        ////get params for db.func
        ///
        //build searchstring
        var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text)
        {
            Value = BuildSearchString(searchstring, false)
        };

        //lookup searchtype string
        var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
        if (searchtypecode >= 0 && searchtypecode <= 3)
        {
            searchtype.Value = st.searchType[searchtypecode.Value];
        }
        else searchtype.Value = st.searchType[3];

        //userid 
        var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = userid
        };

        //if internal call is specified, stored function appsearch won't add to searches/searchhistory
        var internalcall = new NpgsqlParameter("internalcall", NpgsqlTypes.NpgsqlDbType.Boolean)
        {
            Value = true
        };

        //count all matches
        var matchcount = db.Search
            .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search, @internalcall)", appuserid, searchtype,
                search, internalcall)
            .Count();
        System.Console.WriteLine($"{matchcount} results.");

        var page = ISharedRepository.GetPagination(matchcount, pagingAttributes);

        System.Console.WriteLine($"{page} page trying to get.");

        //get subset of results according to pagesize etc
        var resultlist = db.Search
            .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();

        //build and map results to posts
        var resultposts = new List<Posts>();

        foreach (var s in resultlist)
        {
            var p = new Posts();
            var sp = _sharedRepositoryService.GetPost(s.PostId);

            p.ParentId = sp.QuestionId;
            p.Id = sp.Id;
            var endpos = 100;
            if (sp.Body.Length < 100)
            {
                endpos = sp.Body.Length;
            }

            p.Body = sp.Body.Substring(0, endpos);

            p.Title = _questionRepository.GetQuestion(p.ParentId).Title;
            p.TotalResults = matchcount;
            p.Rank = s.Rank;
            resultposts.Add(p);
        }

        return resultposts;
    }


    public IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults)
    {
        // for performing searches with wordrank on the db
        // do actual search using appsearch in db and build results

        // need db context and searchtype lookuptable
        using var db = _dbContextFactory.CreateDbContext();
        var st = new SearchTypeLookupTable();

        // get params for db.func
        // build searchstring
        var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text)
        {
            Value = BuildSearchString(searchstring, false)
        };

        // lookup searchtype string
        var searchType = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
        if (searchtypecode >= 4 && searchtypecode <= 5)
        {
            searchType.Value = st.searchType[searchtypecode];
        }
        else searchType.Value = st.searchType[5];

        // userid 
        var appUserId = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = userid
        };

        // if internal call is specified, stored function appsearch won't add to searches/searchhistory
        var internalCall = new NpgsqlParameter("internalcall", NpgsqlTypes.NpgsqlDbType.Boolean)
        {
            Value = true
        };

        // count all matches
        var matchCount = db.Search
            .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search, @internalcall)", appUserId, searchType,
                search, internalCall)
            .Count();
        Console.WriteLine($"{matchCount} results.");

        var limit = new NpgsqlParameter("limit", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = 1000
        };
        if (maxresults != null)
        {
            limit.Value = maxresults;
        }

        // call db.func wordrank
        return db.WordRank
            .FromSqlRaw("SELECT * from wordrank(@appuserid, @searchtype, @search) limit @limit", appUserId,
                searchType, search, limit)
            .ToList();
    }

    public string BuildSearchString(string searchstring, bool reverse)
    {
        // convert query search string to appsearch db func search string or the reverse
        string[] separators = { ",", ".", "...", " " };

        var words = searchstring.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        System.Console.WriteLine($"{words.Length} tokens in search");

        //added to filter non-aplhanumeric chars
        //better to have it at backend if some1 sends weird request :)
        var filteredTokens = new List<string>();
        foreach (var s in words)
        {
            var filterArray = s.ToCharArray();
            filterArray = Array.FindAll<char>(filterArray, (c => (char.IsLetterOrDigit(c)
                                                                  || char.IsWhiteSpace(c)
                                                                  || c == '-')));
            var filteredToken = new string(filterArray);
            filteredTokens.Add(filteredToken);
        }

        var finalString = string.Join(reverse ? "," : " ", filteredTokens);

        Console.WriteLine("Built search string: " + finalString);
        return finalString;
    }

    public int SearchTypeLookup(string searchmethod)
    {
        //get stype from string methodname
        var st = new SearchTypeLookupTable();
        var stype = Array.FindIndex(st.searchType, s => s.Equals(searchmethod));
        return stype;
    }
}
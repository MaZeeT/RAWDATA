using System.Text.RegularExpressions;
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
        using var db = _dbContextFactory.CreateDbContext();

        // count all matches
        var matchcount = MatchCount(db, userid, searchtypecode, BuildSearchString(searchstring, false));

        var page = ISharedRepository.GetPagination(matchcount, pagingAttributes);

        Console.WriteLine($"{page} page trying to get.");

        // get subset of results according to pagesize etc
        var resultlist = SearchResults(db, userid, searchtypecode, BuildSearchString(searchstring, false), page, pagingAttributes); 

        // build and map results to posts
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
        using var db = _dbContextFactory.CreateDbContext();
        var st = new SearchTypeLookupTable();
        
        var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text)
        {
            Value = BuildSearchString(searchstring, false)
        };
        
        var searchType = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
        if (searchtypecode >= 4 && searchtypecode <= 5)
        {
            searchType.Value = st.searchType[searchtypecode];
        }
        else searchType.Value = st.searchType[5];
        
        var appUserId = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = userid
        };

        var limit = new NpgsqlParameter("limit", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = 1000
        };
        if (maxresults != null)
        {
            limit.Value = maxresults;
        }
        
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
        Console.WriteLine($"{words.Length} tokens in search");

        // added to filter non-aplhanumeric chars
        // better to have it at backend if some1 sends weird request :)
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
        // get stype from string methodname
        var st = new SearchTypeLookupTable();
        var stype = Array.FindIndex(st.searchType, s => s.Equals(searchmethod));
        return stype;
    }

    private int MatchCount(DatabaseContext2 db, int userid, int? searchtypecode, string searchString)
    {
        var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = userid
        };
        
        var searchTypeLookupTable = new SearchTypeLookupTable();
        
        var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text)
        {
            Value = searchString
        };
        
        // if internal call is specified, stored function appsearch won't add to searches/searchhistory
        var internalcall = new NpgsqlParameter("internalcall", NpgsqlTypes.NpgsqlDbType.Boolean)
        {
            Value = true
        };
        
        var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
        switch (searchtypecode)
        {
            case 0:
                searchtype.Value = searchTypeLookupTable.searchType[0];
                string[] tokens = Regex.Split(searchString, @"\s+");
                return SearchAlgorithms.Tfidf.Count(db, tokens);
                return db.Search
                    .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search, @internalcall)", appuserid, searchtype,
                        search, internalcall)
                    .Count();
            case 1:
                searchtype.Value = searchTypeLookupTable.searchType[1];
                return db.Search
                    .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search, @internalcall)", appuserid, searchtype,
                        search, internalcall)
                    .Count();
            case 2:
                searchtype.Value = searchTypeLookupTable.searchType[2];
                return db.Search
                    .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search, @internalcall)", appuserid, searchtype,
                        search, internalcall)
                    .Count();
            case 3:
            default:
                searchtype.Value = searchTypeLookupTable.searchType[3];
                // count all matches
                return db.Search
                    .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search, @internalcall)", appuserid, searchtype,
                        search, internalcall)
                    .Count();
        }
    }

    private List<Search> SearchResults(DatabaseContext2 db, int userid, int? searchtypecode, string searchString, int page, PagingAttributes pagingAttributes)
    {
        var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = userid
        };
        
        var searchTypeLookupTable = new SearchTypeLookupTable();
        
        var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text)
        {
            Value = searchString
        };
        
        List<Search> resultList;
        
        var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
        switch (searchtypecode)
        {
            case 0: {}
                searchtype.Value = searchTypeLookupTable.searchType[0];
                InsertSearchToLogTabel(db, userid, (string)searchtype.Value, searchString);
                string[] tokens = Regex.Split(searchString, @"\s+");
                resultList = SearchAlgorithms.Tfidf.List(db, tokens);
                break;
            case 1:
                searchtype.Value = searchTypeLookupTable.searchType[1];
                InsertSearchToLogTabel(db, userid, (string)searchtype.Value, searchString);
                
                resultList = SearchAlgorithms.ExactMatch(db, appuserid, searchtype, search, page, pagingAttributes);
                
                break;
            case 2:
                searchtype.Value = searchTypeLookupTable.searchType[2];
                InsertSearchToLogTabel(db, userid, (string)searchtype.Value, searchString);

                resultList = SearchAlgorithms.SimpleSearch(db, appuserid, searchtype, search, page, pagingAttributes);

                break;
            case 3:
            default:
                searchtype.Value = searchTypeLookupTable.searchType[3];
                InsertSearchToLogTabel(db, userid, (string)searchtype.Value, searchString);
                
                resultList = SearchAlgorithms.BestMatch(db, appuserid, searchtype, search, page, pagingAttributes);
                break;
        }
        
        return resultList;
    }

    private void InsertSearchToLogTabel(DatabaseContext2 db, int userid, string searchtype, string searchString)
    {
        // Insert search to search log
        var searches = new Searches
        {
            Id = 0,
            UserId = userid,
            SearchType = searchtype,
            SearchString = searchString,
            Date = default
        };
        db.Searches.Add(searches);
        db.SaveChanges();
    }
    
    private NpgsqlParameter GetTokens(string searchString)
    {
        // Split searchstring to array of words
        string[] tokens = Regex.Split(searchString, @"\s+");
        
        var tokenParameter = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text)
        {
            Value = tokens
        };
        
        return tokenParameter;
    }
}
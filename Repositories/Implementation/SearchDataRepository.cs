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
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;
    private readonly IQuestionRepository _questionRepository;
    private readonly ISharedRepository _sharedRepositoryService;

    public SearchDataRepository(IDbContextFactory<DatabaseContext> factory, IQuestionRepository questionRepository,
        ISharedRepository sharedRepositoryService)
    {
        _dbContextFactory = factory;
        _questionRepository = questionRepository;
        _sharedRepositoryService = sharedRepositoryService;
    }

    public IList<Posts> Search(int userid, string searchString, int? searchTypeCode,
        PagingAttributes pagingAttributes)
    {
        using var db = _dbContextFactory.CreateDbContext();

        // count all matches
        var matchCount = MatchCount(db, searchTypeCode, BuildSearchString(searchString, false));

        var page = ISharedRepository.GetPagination(matchCount, pagingAttributes);

        Console.WriteLine($"{page} page trying to get.");

        // get subset of results according to pagesize etc
        var resultList = SearchResults(db, userid, searchTypeCode, BuildSearchString(searchString, false), page, pagingAttributes); 

        // build and map results to posts
        var resultPosts = new List<Posts>();

        foreach (var s in resultList)
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
            p.TotalResults = matchCount;
            p.Rank = s.Rank;
            resultPosts.Add(p);
        }

        return resultPosts;
    }


    public IList<WordRank> WordRank(int userid, string searchString, int searchTypeCode, int? maxResults)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var searchTypeLookupTable = new SearchTypeLookupTable();
        
        var searchType = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
        if (searchTypeCode >= 4 && searchTypeCode <= 5)
        {
            searchType.Value = searchTypeLookupTable.searchType[searchTypeCode];
        }
        else searchType.Value = searchTypeLookupTable.searchType[5];

        var resultLimit = maxResults ?? 1000;
        
        InsertSearchToLogTable(db, userid, searchTypeLookupTable.searchType[searchTypeCode], searchString);
        string[] words = Regex.Split(searchString, @"\s+");
        switch (searchTypeLookupTable.searchType[searchTypeCode])
        {
            case "wordstfidf":
                return WordRankTfidf(db, words, resultLimit);
            case "wordsbest":
                return WordRankBest(db, words, resultLimit);
            default:
                throw new ArgumentException("Invalid search type");
        }
    }

    public string BuildSearchString(string searchString, bool reverse)
    {
        string[] separators = [",", ".", "...", " "];

        var words = searchString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"{words.Length} tokens in search");

        // added to filter non-alphanumeric chars
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

    public int SearchTypeLookup(string searchType)
    {
        // get search type from string method name
        var st = new SearchTypeLookupTable();
        return Array.FindIndex(st.searchType, s => s.Equals(searchType));
    }

    private static int MatchCount(DatabaseContext db, int? searchTypeCode, string searchString)
    {
        string[] tokens = Regex.Split(searchString, @"\s+");
        
        switch (searchTypeCode)
        {
            case 0:
                return SearchAlgorithms.Tfidf.Count(db, tokens);

            case 1:
                return SearchAlgorithms.ExactMatch.Count(db, tokens);

            case 2:
                return SearchAlgorithms.SimpleSearch.Count(db, tokens);

            case 3:
            default:
                return SearchAlgorithms.BestMatch.Count(db, tokens);
        }
    }

    private static List<Search> SearchResults(DatabaseContext db, int userid, int? searchtypecode, string searchString, int page, PagingAttributes pagingAttributes)
    {
        
        var searchTypeLookupTable = new SearchTypeLookupTable();

        List<Search> resultList;
        
        string[] tokens = Regex.Split(searchString, @"\s+");
        
        switch (searchtypecode)
        {
            case 0:
                InsertSearchToLogTable(db, userid, searchTypeLookupTable.searchType[0], searchString);
                resultList = SearchAlgorithms.Tfidf.List(db, tokens);
                break;
            case 1:
                InsertSearchToLogTable(db, userid, searchTypeLookupTable.searchType[1], searchString);
                resultList = SearchAlgorithms.ExactMatch.List(db, tokens);
                break;
            case 2:
                InsertSearchToLogTable(db, userid, searchTypeLookupTable.searchType[2], searchString);
                resultList = SearchAlgorithms.SimpleSearch.List(db, tokens);
                break;
            case 3:
            default:
                InsertSearchToLogTable(db, userid, searchTypeLookupTable.searchType[3], searchString);
                resultList = SearchAlgorithms.BestMatch.List(db, tokens);
                break;
        }
        
        return resultList;
    }

    private static void InsertSearchToLogTable(DatabaseContext db, int userid, string searchtype, string searchString)
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
    
    private IList<WordRank> WordRankTfidf(DatabaseContext db, string[] words, int limit)
    {
        // Build the UNION ALL equivalent
        var weightedQuery = words
            .Select(w =>
                db.WiWeighted
                    .Where(x => x.Word == w && (x.What == "title" || x.What == "body"))
                    .Select(x => new { x.Id, x.Tfidf })
            )
            .Aggregate((a, b) => a.Concat(b));

        // Sum TFIDF per id
        var perDoc = weightedQuery
            .GroupBy(x => x.Id)
            .Select(g => new { Id = g.Key, Rank = g.Sum(x => x.Tfidf) });

        // Join wi and group per word
        var result = db.WiWeighted
            .Join(perDoc, wi => wi.Id, d => d.Id, (wi, d) => new { wi.Word, d.Rank })
            .GroupBy(x => x.Word)
            .Select(g => new WordRank
            {
                Term = g.Key,
                Rank = (decimal)g.Sum(x => x.Rank)
            })
            .OrderByDescending(x => x.Rank)
            .Take(limit)
            .ToList();

        return result;
    }
    
    private IList<WordRank> WordRankBest(DatabaseContext db, string[] words, int limit)
    {
        // Build UNION ALL for wi
        var baseQuery = words
            .Select(w =>
                db.WiWeighted
                    .Where(x => x.Word == w)
                    .Select(x => new { x.Id, Relevance = 1 })
            )
            .Aggregate((a, b) => a.Concat(b));

        var perDoc = baseQuery
            .GroupBy(x => x.Id)
            .Select(g => new { Id = g.Key, Rank = g.Sum(x => x.Relevance) });

        var result = db.WiWeighted
            .Join(perDoc, wi => wi.Id, d => d.Id, (wi, d) => new { wi.Word, d.Rank })
            .GroupBy(x => x.Word)
            .Select(g => new WordRank
            {
                Term = g.Key,
                Rank = g.Sum(x => x.Rank)
            })
            .OrderByDescending(x => x.Rank)
            .Take(limit)
            .ToList();

        return result;
    }
}
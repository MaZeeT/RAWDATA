using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories;

public class SearchDataRepository : ISearchRepository
{
    private readonly DatabaseContext _dbContext;
    private readonly IQuestionRepository _questionRepository;
    private readonly ISharedRepository _sharedRepositoryService;

    public SearchDataRepository(DatabaseContext dbContext, IQuestionRepository questionRepository,
        ISharedRepository sharedRepositoryService)
    {
        _dbContext = dbContext;
        _questionRepository = questionRepository;
        _sharedRepositoryService = sharedRepositoryService;
    }

    public IList<Posts> Search(int userid, string searchString, SearchType searchType,
        PagingAttributes pagingAttributes)
    {
        using var db = _dbContext;

        // count all matches
        var matchCount = MatchCount(db, searchType, BuildSearchString(searchString, false));

        var page = ISharedRepository.GetPagination(matchCount, pagingAttributes);

        Console.WriteLine($"{page} page trying to get.");

        // get subset of results according to pagesize etc
        var resultList = SearchResults(db, userid, searchType, BuildSearchString(searchString, false), page, pagingAttributes); 

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


    public IList<WordRank> WordRank(int userid, string searchString, SearchType searchTyper, int? maxResults)
    {
        using var db = _dbContext;
        
        var resultLimit = maxResults ?? 1000;
        
        InsertSearchToLogTable(db, userid, searchTyper, searchString);
        string[] words = Regex.Split(searchString, @"\s+");
        switch (searchTyper)
        {
            case SearchType.WordsTfidf:
                return WordRankTfidf(db, words, resultLimit);
            case SearchType.WordsBest:
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

    private static int MatchCount(DatabaseContext db, SearchType searchType, string searchString)
    {
        string[] tokens = Regex.Split(searchString, @"\s+");
        
        switch (searchType)
        {
            case SearchType.Tfidf:
                return SearchAlgorithms.Tfidf.Count(db, tokens);

            case SearchType.ExactMatch:
                return SearchAlgorithms.ExactMatch.Count(db, tokens);

            case SearchType.Simple:
                return SearchAlgorithms.SimpleSearch.Count(db, tokens);

            case SearchType.BestMatch:
            default:
                return SearchAlgorithms.BestMatch.Count(db, tokens);
        }
    }

    private static List<Search> SearchResults(DatabaseContext db, int userid, SearchType searchType, string searchString, int page, PagingAttributes pagingAttributes)
    {
        List<Search> resultList;
        
        string[] tokens = Regex.Split(searchString, @"\s+");
        
        switch (searchType)
        {
            case SearchType.Tfidf:
                InsertSearchToLogTable(db, userid, SearchType.Tfidf, searchString);
                resultList = SearchAlgorithms.Tfidf.List(db, tokens);
                break;
            case SearchType.ExactMatch:
                InsertSearchToLogTable(db, userid, SearchType.ExactMatch, searchString);
                resultList = SearchAlgorithms.ExactMatch.List(db, tokens);
                break;
            case SearchType.Simple:
                InsertSearchToLogTable(db, userid, SearchType.Simple, searchString);
                resultList = SearchAlgorithms.SimpleSearch.List(db, tokens);
                break;
            case SearchType.BestMatch:
            default:
                InsertSearchToLogTable(db, userid, SearchType.BestMatch, searchString);
                resultList = SearchAlgorithms.BestMatch.List(db, tokens);
                break;
        }
        
        return resultList;
    }

    private static void InsertSearchToLogTable(DatabaseContext db, int userid, SearchType searchtype, string searchString)
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
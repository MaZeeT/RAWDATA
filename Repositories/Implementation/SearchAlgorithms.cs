using Domain.Models;
using Domain.Services;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Repositories.Implementation;

public static class SearchAlgorithms
{
    public static class Tfidf
    {
        internal static List<Search> List(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).ToList();
        }

        internal static int Count(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).Count();
        }
    
        private static IEnumerable<Search> Query(DatabaseContext2 db, string[] searchWords)
        {
            // Base query from wi_weighted table
            var query = db.WiWeighted
                .Where(w => (w.What == "title" || w.What == "body") 
                            && searchWords.Contains(w.Word));

            // Step 1: Let EF compute the sum (no rounding yet)
            var intermediate =  query
                .GroupBy(w => w.Id)
                .Select(g => new
                {
                    PostId = g.Key,
                    Rank = (double)(g.Sum(x => x.Tfidf) ?? 0)
                })
                .OrderByDescending(r => r.Rank)
                .ToList();
        
            // Step 2: Perform rounding in memory (safe and unambiguous)
            var results = intermediate
                .Select(r => new Search
                {
                    PostId = r.PostId,
                    Rank = Math.Round(r.Rank, 4)
                });

            return results;
        }
    }

    public static class ExactMatchClass {}

    public static class SimpleSearch
    {
        internal static List<Search> List(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).ToList();
        }

        internal static int Count(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).Count();
        }
    
        private static IQueryable<Search> Query(DatabaseContext2 db, string[] searchWords)
        {
            if (searchWords.Length == 0)
                return new List<Search>().AsQueryable();
            
            var keyword = searchWords[0];

            // Wrap keyword with % for substring search (ILike = case-insensitive)
            var pattern = $"%{keyword}%";

            var questionMatches = db.Questions
                .Where(q =>
                    EF.Functions.ILike(q.Title, pattern) ||
                    EF.Functions.ILike(q.Body, pattern))
                .Select(q => new Search
                {
                    PostId = q.Id,
                    Rank = (double)0m
                });

            var answerMatches = db.Answers
                .Where(a => EF.Functions.ILike(a.Body, pattern))
                .Select(a => new Search
                {
                    PostId = a.Id,
                    Rank = (double)0m
                });

            // UNION ALL equivalent
            return questionMatches
                .Union(answerMatches);
        }
    }
    
    public static class BestMatchClass {}
    
   /* public static class ExactMatch
    {
        internal static List<Search> List(DatabaseContext2 db, string[] searchWords)
        {
            return ExactMatch(db, searchWords).ToList();
        }

        internal static int Count(DatabaseContext2 db, string[] searchWords)
        {
            return ExactMatch(db, searchWords).Count();
        }

        private static IEnumerable<Search> ExactMatch(DatabaseContext2 db, string[] searchWords)
        {
            db.Search
                .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
        }
    }*/
    
    internal static List<Search> ExactMatch(DatabaseContext2 db, NpgsqlParameter appuserid, NpgsqlParameter searchtype, NpgsqlParameter search, int page, PagingAttributes pagingAttributes)
    {
                        
        var resultList = db.Search
            .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
        
        return resultList;
    }
    
    internal static List<Search> BestMatch(DatabaseContext2 db, NpgsqlParameter appuserid, NpgsqlParameter searchtype, NpgsqlParameter search, int page, PagingAttributes pagingAttributes)
    {
                        
        var resultList = db.Search
            .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
        
        return resultList;
    }
}
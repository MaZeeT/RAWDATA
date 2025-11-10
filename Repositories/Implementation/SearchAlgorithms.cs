using Domain.Models;
using Domain.Services;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Repositories.Implementation;

public static class SearchAlgorithms
{
    internal static List<Search> tfidf(DatabaseContext2 db, string[] searchWords)
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
            })
            .ToList();

        return results;
    }
    
    internal static List<Search> ExactMatch(DatabaseContext2 db, NpgsqlParameter appuserid, NpgsqlParameter searchtype, NpgsqlParameter search, int page, PagingAttributes pagingAttributes)
    {
                        
        var resultList = db.Search
            .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
        
        return resultList;
    }
    internal static List<Search> SimpleSearch(DatabaseContext2 db, NpgsqlParameter appuserid, NpgsqlParameter searchtype, NpgsqlParameter search, int page, PagingAttributes pagingAttributes)
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
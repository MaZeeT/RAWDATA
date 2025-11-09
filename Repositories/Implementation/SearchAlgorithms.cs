using Domain.Models;
using Domain.Services;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Repositories.Implementation;

public static class SearchAlgorithms
{
    internal static List<Search> tfidf(DatabaseContext2 db, NpgsqlParameter appuserid, NpgsqlParameter searchtype, NpgsqlParameter search, int page, PagingAttributes pagingAttributes)
    {
                        
        var resultList = db.Search
            .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
        
        return resultList;
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
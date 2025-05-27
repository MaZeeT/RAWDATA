using Infrastructure.Database;
using Domain.AnnotationsDTOs;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class AnnotationRepository : IAnnotationRepository
{
    private readonly IDbContextFactory<DatabaseContext2> _dbContextFactory;

    public AnnotationRepository(IDbContextFactory<DatabaseContext2> factory)
    {
        _dbContextFactory = factory;
    }
    
    /// <summary>
    /// Create annotation without function, simple, raw, need to know HistoryId
    /// </summary>
    /// <param name="annotationObject"></param>
    /// <returns></returns>
    public Annotations CreateAnnotations(AnnotationsDto annotationObject)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var annotation = new Annotations
        {
            UserId = annotationObject.UserId,
            HistoryId = annotationObject.HistoryId,
            Body = annotationObject.Body,
            Date = annotationObject.Date
        };
        db.Annotations.Add(annotation);
        db.SaveChanges();
        return GetAnnotation(annotation.Id);
    }

    /// <summary>
    /// Returns annotation found only by annotationId
    /// </summary>
    /// <param name="annotationId"></param>
    /// <returns>Annotations Type Object</returns>
    public Annotations GetAnnotation(int annotationId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var result = db.Annotations.Find(annotationId);

        return result;
    }

    /// <summary>
    /// Returns annotation found by annotationId and userId
    /// </summary>
    /// <param name="annotationId"></param>
    /// <param name="userId"></param>
    /// <returns>Annotations Type Object</returns>
    public Annotations GetAnnotationByUserId(int annotationId, int userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var result = db.Annotations
            .Where(a => a.UserId == userId)
            .FirstOrDefault(a => a.Id == annotationId);
        return result;
    }

    /// <summary>
    /// Gets all the annotations of a userId and a postId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="postId"></param>
    /// <param name="pagingAttributes"></param>
    /// <returns>List Type SimpleAnnotationsDto</returns>
    public List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId,
        PagingAttributes pagingAttributes)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var page = ISharedRepository.GetPagination(UserAnnotOnPostListCount(userId, postId), pagingAttributes);
        var annotationsOfPostList = (from annot in db.Annotations
                join hist in db.History on annot.HistoryId equals hist.Id
                where hist.Postid == postId && annot.UserId == userId
                select new SimpleAnnotationDto
                {
                    AnnotationId = annot.Id,
                    Body = annot.Body,
                    Date = annot.Date
                }).Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
        return annotationsOfPostList;
    }

    public int UserAnnotOnPostListCount(int userId, int postId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var annotationsCount = from annot in db.Annotations
            join hist in db.History on annot.HistoryId equals hist.Id
            where annot.UserId == userId && hist.Postid == postId
            group annot by annot.Id
            into tot
            select tot.Count();
        return annotationsCount.FirstOrDefault();
    }


    /// <summary>
    /// Returns a list of annotations and their postId recorded in history table
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="pagingAttributes"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<PostAnnotationsDto> GetAllAnnotationsOfUser(int userId, PagingAttributes pagingAttributes,
        out int count)
    {
        using var db = _dbContextFactory.CreateDbContext();
        count = GetAllAnnotationsOfUserCount(userId);
        var page = ISharedRepository.GetPagination(count, pagingAttributes);

        var result = (from annot in db.Annotations
                join hist in db.History on annot.HistoryId equals hist.Id
                where annot.UserId == userId
                select new PostAnnotationsDto
                {
                    AnnotationId = annot.Id,
                    PostId = hist.Postid,
                    Body = annot.Body,
                    Date = annot.Date
                }).Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
        return result;
    }

    public int GetAllAnnotationsOfUserCount(int userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var listCount = (from annot in db.Annotations
            join hist in db.History on annot.HistoryId equals hist.Id
            where annot.UserId == userId
            select annot).Count();
        
        return listCount;
    }

    /// <summary>
    /// Deletes selected annotation of annotationId of a userId 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns>boolean</returns>
    public bool DeleteAnnotation(int id, int userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        try
        {
            var itemToDelete = GetAnnotationByUserId(id, userId);
            db.Annotations.Remove(itemToDelete);
            db.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateAnnotation_withFunction(AnnotationsDto newAnnotation, out int newId)
    {
        try
        {
            using var db = _dbContextFactory.CreateDbContext();

            var userId = new NpgsqlParameter("userid", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = newAnnotation.UserId
            };
            var postId = new NpgsqlParameter("postid", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = newAnnotation.PostId
            };
            var annotationBody = new NpgsqlParameter("body", NpgsqlTypes.NpgsqlDbType.Text)
            {
                Value = newAnnotation.Body
            };

            // since this select annotate function runs with select as Id and is attached to the AnnotateFunction Dto and returns only 1 result
            // it is ok to .FirstOrDefult() and then .Id to get the value directly. 
            newId = db.AnnotateFunction
                .FromSqlRaw("select annotate(@userid, @postid, @body) as Id", userId, postId, annotationBody)
                .FirstOrDefault()
                .Id;
            db.SaveChanges();
            //if the returned id is somehow weird and the annotation is not found, then annotationFromDb gets null here
            return true;
        }
        catch (Exception)
        {
            newId = -1;
            return false;
        }
    }

    public bool UpdateAnnotation(int annotationId, string annotationBody)
    {
        using var db = _dbContextFactory.CreateDbContext();
        try
        {
            var annotationToUpdate = db.Annotations.Find(annotationId);
            annotationToUpdate.Body = annotationBody;
            db.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
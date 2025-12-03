using Infrastructure.Database;
using Domain.AnnotationsDTOs;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class AnnotationRepository : IAnnotationRepository
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public AnnotationRepository(IDbContextFactory<DatabaseContext> factory)
    {
        _dbContextFactory = factory;
    }
    
    public Annotations GetAnnotation(int annotationId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return db.Annotations.Find(annotationId) ?? throw new KeyNotFoundException("Annotation not found");
    }
    
    private Annotations GetAnnotationByUserId(int annotationId, int userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var result = db.Annotations
            .Where(a => a.UserId == userId)
            .FirstOrDefault(a => a.Id == annotationId);
        return result ?? throw new KeyNotFoundException("Annotation not found");
    }

    public List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId,
        PagingAttributes pagingAttributes)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var page = ISharedRepository.GetPagination(UserAnnotOnPostListCount(userId, postId), pagingAttributes);
        var annotationsOfPostList = (from annot in db.Annotations
                join hist in db.History on annot.HistoryId equals hist.Id
                where hist.PostId == postId && annot.UserId == userId
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

    private int UserAnnotOnPostListCount(int userId, int postId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var annotationsCount = from annot in db.Annotations
            join hist in db.History on annot.HistoryId equals hist.Id
            where annot.UserId == userId && hist.PostId == postId
            group annot by annot.Id
            into tot
            select tot.Count();
        return annotationsCount.FirstOrDefault();
    }
    
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
                    PostId = hist.PostId,
                    Body = annot.Body,
                    Date = annot.Date
                }).Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
        return result;
    }

    private int GetAllAnnotationsOfUserCount(int userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var listCount = (from annot in db.Annotations
            join hist in db.History on annot.HistoryId equals hist.Id
            where annot.UserId == userId
            select annot).Count();
        
        return listCount;
    }
    
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
    
    public bool AddAnnotation(AnnotationsDto newAnnotation, out int newId)
    {
        try
        {
            using var db = _dbContextFactory.CreateDbContext();
            
            var conn = db.Database.GetDbConnection();
            db.Database.OpenConnection();
            
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            INSERT INTO annotations (userid, historyid, body, date)
            VALUES (@userid, @postid, @body, CURRENT_TIMESTAMP)
            RETURNING id;";

            // Create and add parameters
            var userIdParam = cmd.CreateParameter();
            userIdParam.ParameterName = "userid";
            userIdParam.Value = newAnnotation.UserId;
            cmd.Parameters.Add(userIdParam);

            var postIdParam = cmd.CreateParameter();
            postIdParam.ParameterName = "postid";
            postIdParam.Value = newAnnotation.PostId;
            cmd.Parameters.Add(postIdParam);

            var bodyParam = cmd.CreateParameter();
            bodyParam.ParameterName = "body";
            bodyParam.Value = newAnnotation.Body;
            cmd.Parameters.Add(bodyParam);

            // Execute and read the ID
            var result = cmd.ExecuteScalar();
            newId = result != null ? Convert.ToInt32(result) : -1;

            return newId != -1;
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
using Application.Interfaces;
using Infrastructure.Database;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Implementation;

public class AnnotationRepository : IAnnotationRepository
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public AnnotationRepository(IDbContextFactory<DatabaseContext> factory)
    {
        _dbContextFactory = factory;
    }
    
    public Annotations? GetAnnotation(int annotationId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return db.Annotations.Find(annotationId);
    }
    
    private Annotations? GetAnnotationByUserId(int annotationId, int userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var result = db.Annotations
            .Where(a => a.UserId == userId)
            .FirstOrDefault(a => a.Id == annotationId);
        return result;
    }

    public List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId,
        PagingAttributes pagingAttributes)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var page = ISharedRepository.GetPagination(UserAnnotOnPostListCount(userId, postId), pagingAttributes);

        var query =
            from annotation in db.Annotations
            join history in db.History on annotation.HistoryId equals history.Id
            where annotation.HistoryId == postId && annotation.UserId == userId
            select new SimpleAnnotationDto();
        
            return query
                .Skip(page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();
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
        
        var result = db.Annotations
            .Where(a => a.UserId == userId)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .Select(a => new PostAnnotationsDto
            {
                AnnotationId = a.Id,
                PostId = a.History != null ? a.History.PostId : null,
                Body = a.Body,
                Date = a.Date
            })
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
            db.Annotations.Remove(itemToDelete ?? throw new InvalidOperationException($"Annotation not found for deletion, with id {id}"));
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

            var annotation = new Annotations
            {
                UserId = newAnnotation.UserId,
                HistoryId = newAnnotation.PostId,
                Body = newAnnotation.Body,
                Date = DateTime.UtcNow
            };

            db.Annotations.Add(annotation);
            db.SaveChanges();

            newId = annotation.Id; // EF automatically populates this
            return true;
        }
        catch
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
            annotationToUpdate?.Body = annotationBody;
            db.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
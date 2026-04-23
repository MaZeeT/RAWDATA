using System;
using System.Collections.Generic;
using System.Linq;
using Application.Interfaces.Repositories;
using Domain.DTO;
using Domain.Entities;
using Infrastructure.DataAccess.Database;

namespace Infrastructure.DataAccess.Repositories;

public class AnnotationRepository : IAnnotationRepository
{
    private readonly DatabaseContext _dbContext;

    public AnnotationRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Annotations? GetAnnotation(int annotationId)
    {
        return _dbContext.Annotations.Find(annotationId);
    }

    public List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId,
        PagingAttributes pagingAttributes)
    {
        var userAnnotationCount = from annot in _dbContext.Annotations
            join hist in _dbContext.History on annot.HistoryId equals hist.Id
            where annot.UserId == userId && hist.PostId == postId
            group annot by annot.Id
            into tot
            select tot.Count();
        
        var page = ISharedRepository.GetPagination(userAnnotationCount.Count(), pagingAttributes);

        var query =
            from annotation in _dbContext.Annotations
            join history in _dbContext.History on annotation.HistoryId equals history.Id
            where annotation.HistoryId == postId && annotation.UserId == userId
            select new SimpleAnnotationDto();

        return query
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
    }
    
    public List<PostAnnotationsDto> GetAllAnnotationsOfUser(int userId, PagingAttributes pagingAttributes,
        out int count)
    {
        count = (from annot in _dbContext.Annotations
            join hist in _dbContext.History on annot.HistoryId equals hist.Id
            where annot.UserId == userId
            select annot).Count(); 
            
        var page = ISharedRepository.GetPagination(count, pagingAttributes);

        var result = _dbContext.Annotations
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
    
    public bool DeleteAnnotation(int id, int userId)
    {
        try
        {
            var itemToDelete = _dbContext.Annotations
                .Where(a => a.UserId == userId)
                .FirstOrDefault(a => a.Id == id); 
            
            _dbContext.Annotations.Remove(itemToDelete ??
                                          throw new InvalidOperationException(
                                              $"Annotation not found for deletion, with id {id}"));
            _dbContext.SaveChanges();
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
    
            var annotation = new Annotations
            {
                UserId = newAnnotation.UserId,
                HistoryId = newAnnotation.PostId,
                Body = newAnnotation.Body,
                Date = DateTime.UtcNow
            };

            _dbContext.Annotations.Add(annotation);
            _dbContext.SaveChanges();

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
        try
        {
            var annotationToUpdate = _dbContext.Annotations.Find(annotationId);
            annotationToUpdate?.Body = annotationBody;
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
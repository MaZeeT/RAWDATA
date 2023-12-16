using DatabaseService.Modules;
using DatabaseService.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseService.Services;

public class AnnotationService : IAnnotation
{
    /// <summary>
    /// Create annotation without function, simple, raw, need to know HistoryId
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Annotations CreateAnnotations(AnnotationsDto obj)
    {
        using var DB = new DatabaseContext();
        var annotation = new Annotations
        {
            UserId = obj.UserId,
            HistoryId = obj.HistoryId,
            Body = obj.Body,
            Date = obj.Date
        };
        DB.Annotations.Add(annotation);
        DB.SaveChanges();
        return GetAnnotation(annotation.Id);
    }

    /// <summary>
    /// Returns annotation found only by annotationId
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Annotations Type Object</returns>
    public Annotations GetAnnotation(int value)
    {
        using var DB = new DatabaseContext();
        var result = DB.Annotations.Find(value);

        return result;
    }

    /// <summary>
    /// Returns annotation found by annotationId and userId
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns>Annotations Type Object</returns>
    public Annotations GetAnnotationByUserId(int id, int userId)
    {
        using var DB = new DatabaseContext();
        var result = DB.Annotations
            .Where(a => a.UserId == userId)
            .Where(a => a.Id == id).FirstOrDefault();
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
        using var DB = new DatabaseContext();
        var sharedService = new SharedService();
        var page = sharedService.GetPagination(UserAnnotOnPostListCount(userId, postId), pagingAttributes);
        var annotationsOfPostList = (from annot in DB.Annotations
                join hist in DB.History on annot.HistoryId equals hist.Id
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
        using var DB = new DatabaseContext();
        var annotationsCount = from annot in DB.Annotations
            join hist in DB.History on annot.HistoryId equals hist.Id
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
    /// <param name="postId"></param>
    /// <returns></returns>
    public List<PostAnnotationsDto> GetAllAnnotationsOfUser(int userId, PagingAttributes pagingAttributes,
        out int count)
    {
        using var DB = new DatabaseContext();
        var sharedService = new SharedService();
        count = GetAllAnnotationsOfUserCount(userId);
        var page = sharedService.GetPagination(count, pagingAttributes);

        var result = (from annot in DB.Annotations
                join hist in DB.History on annot.HistoryId equals hist.Id
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
        using var DB = new DatabaseContext();
        var listCount = (from annot in DB.Annotations
            join hist in DB.History on annot.HistoryId equals hist.Id
            where annot.UserId == userId
            select annot).Count();

        var res = listCount;
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
        using var DB = new DatabaseContext();
        try
        {
            var itemToDelete = GetAnnotationByUserId(id, userId);
            DB.Annotations.Remove(itemToDelete);
            DB.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateAnnotation_withFunction(AnnotationsDto obj, out int newId)
    {
        try
        {
            using var DB = new DatabaseContext();

            var userId = new NpgsqlParameter("userid", NpgsqlTypes.NpgsqlDbType.Integer);
            userId.Value = obj.UserId;
            var postId = new NpgsqlParameter("postid", NpgsqlTypes.NpgsqlDbType.Integer);
            postId.Value = obj.PostId;
            var annotationBody = new NpgsqlParameter("body", NpgsqlTypes.NpgsqlDbType.Text);
            annotationBody.Value = obj.Body;

            // since this select annotate function runs with select as Id and is attached to the AnnotateFunction Dto and returns only 1 result
            // it is ok to .FirstOrDefult() and then .Id to get the value directly. 
            newId = DB.AnnotateFunction
                .FromSqlRaw("select annotate(@userid, @postid, @body) as Id", userId, postId, annotationBody)
                .FirstOrDefault()
                .Id;
            DB.SaveChanges();
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
        using var DB = new DatabaseContext();
        try
        {
            var annotationToUpdate = DB.Annotations.Find(annotationId);
            annotationToUpdate.Body = annotationBody;
            DB.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseService.Services
{
    public class AnnotationService : IAnnotationService
    {
        public Annotations CreateAnnotations(AnnotationsDto obj)
        {
            using var DB = new AppContext();
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
        public Annotations GetAnnotation(int value)
        {
            using var DB = new AppContext();
            var result = DB.Annotations.Find(value);
            
            return result;
        }
        public List<AnnotationsDto> GetAllAnnotationsOfUser(int userId, PagingAttributes pagingAttributes)
        {
            using var DB = new AppContext();
            var listAnnotationsOfUser = (from annot in DB.Annotations
                                         join hist in DB.History on annot.HistoryId equals hist.Id
                                         where annot.UserId == userId
                                         select new AnnotationsDto
                                         {
                                             AnnotationId = annot.Id,
                                             PostId = hist.Postid,
                                             Body = annot.Body,
                                             Date = annot.Date
                                         }).ToList();
           
           
            return listAnnotationsOfUser;
        }

        //This gets the normal simple annotation from the db and not the actual post with body and title
        public List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId, PagingAttributes pagingAttributes)
        {
            using var DB = new AppContext();
            
            var annotationsCount = from annot in DB.Annotations
                               join hist in DB.History on annot.HistoryId equals hist.Id
                               where annot.UserId == userId
                               group annot by annot.Id into tot
                               select tot.Count();
            var page = GetPagination(annotationsCount.FirstOrDefault(), pagingAttributes);
            var annotationsOfPostList =  (from annot in DB.Annotations
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


        /// <summary>
        /// Returns a list of annotations and their postId recorded in history table
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <returns></returns>
        public List<PostAnnotationsDto> GetAllAnnotationsOfUser(int userId, int postId, PagingAttributes pagingAttributes)
        {
            using var DB = new AppContext();
            var listCount = from annot in DB.Annotations
                            join hist in DB.History on annot.HistoryId equals hist.Id
                            join quest in DB.Questions on hist.Postid equals quest.Id
                            where hist.Postid == postId && annot.UserId == userId
                            group annot by annot.Id into tot
                            select tot.Count();
            var page = GetPagination(listCount.FirstOrDefault(), pagingAttributes);

            var result = (from annot in DB.Annotations
                          join hist in DB.History on annot.HistoryId equals hist.Id
                          join quest in DB.Questions on hist.Postid equals quest.Id
                          where hist.Postid == postId && annot.UserId == userId
                          select new PostAnnotationsDto
                          {
                              AnnotationId = annot.Id,
                              PostId = postId,
                              Body = annot.Body,
                              Date = annot.Date
                          }).Skip(page * pagingAttributes.PageSize)
                            .Take(pagingAttributes.PageSize)
                            .ToList();

            return result;
        }

        public bool DeleteAnnotation(int id)
        {
            using var DB = new AppContext();
            try
            {
                var itemToDelete = GetAnnotation(id);
                DB.Annotations.Remove(itemToDelete);
                DB.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool CreateAnnotation_withFunction(Annotations obj, out Annotations annotationFromDb)
        {
            try
            {
                using var DB = new AppContext();
               
                var userId = new NpgsqlParameter("userid", NpgsqlTypes.NpgsqlDbType.Integer);
                userId.Value = obj.UserId;
                var postId = new NpgsqlParameter("historyid", NpgsqlTypes.NpgsqlDbType.Integer);
                postId.Value = obj.HistoryId;
                var annotationBody = new NpgsqlParameter("body", NpgsqlTypes.NpgsqlDbType.Text);
                annotationBody.Value = obj.Body;

                // since this select annotate function runs with select as Id and is attached to the AnnotateFunction Dto and returns only 1 result
                // it is ok to .FirstOrDefult() and then .Id to get the value directly. 
                var annotationId = DB.AnnotateFunction
                                        .FromSqlRaw("select annotate(@userid, @historyid, @body) as Id", userId, postId, annotationBody)
                                        .FirstOrDefault()
                                        .Id;
                
                //if the returned id is somehow weird and the annotation is not found, then annotationFromDb gets null here
                annotationFromDb = GetAnnotation(annotationId);
                return true;

            }catch(Exception e)
            {
                annotationFromDb = null;
                return false;
            }
           
        }

        public bool UpdateAnnotation(int annotationId, string annotationBody)
        {
            using var DB = new AppContext();
            try
            {
                var annotationToUpdate = DB.Annotations.Find(annotationId);
                annotationToUpdate.Body = annotationBody;
                DB.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        private int GetPagination(int matchcount, PagingAttributes pagingAttributes)
        {
            //calc max pages and set requested page to last page if out of bounds
            var calculatedNumberOfPages = (int)Math.Ceiling((double)matchcount / pagingAttributes.PageSize) - 1;
            System.Console.WriteLine($"{calculatedNumberOfPages} calculated pages.");
            int page;
            if (pagingAttributes.Page > calculatedNumberOfPages)
            {
                page = calculatedNumberOfPages;
            }

            if (pagingAttributes.Page <= 0)
            {
                page = 0;
            }
            else page = pagingAttributes.Page - 1;
            return page;
        }

    }

}

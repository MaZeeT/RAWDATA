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
        public List<Annotations> GetAllAnnotationsByUserId(int userId)
        {
            using var DB = new AppContext();
            var result = DB.Annotations
                           .Where(x => x.UserId == userId)
                           .ToList();

            return result;
        }

        /*//This gets the normal simple annotation from the db and not the actual post with body and title
        public List<Annotations> GetAnnotationsByPostId(int userId, int postId)
        {
            using var DB = new AppContext();
            var result = DB.Annotations
                           .Where(val => val.UserId == userId)
                           .Where(val => val.HistoryId == postId)
                           .ToList();
            return result;
        }*/


            ///alter this one!!! 
        public List<AnnotationsQuestions> GetAnnotationsAndQuestionsByPostId(int userId, int postId)
        {
            using var DB = new AppContext();
            var result = from annot in DB.Annotations
                         join hist in DB.History on annot.HistoryId equals hist.Id
                         join quest in DB.Questions on hist.PostId equals quest.Id
                         where hist.PostId == postId && annot.UserId == userId
                         select new AnnotationsQuestions
                         {
                             Id = annot.Id,
                             UserId = annot.UserId,
                             HistPostId = annot.HistoryId,
                             AnnotationCreationDate = annot.Date,
                             QuestionId = quest.Id,
                             QuestionTitle = quest.Title,
                             QuestionBody = quest.Body,
                             IsBookmark = hist.IsBookmark
                         };

            return result.ToList();
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

    }

}

using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Linq;

namespace DatabaseService.Services
{
    public class AppUsersDataService : IAppUsersDataService
    {
        public Annotations CreateAnnotations(AnnotationsDto obj)
        {
            using var DB = new AppContext();
            var nextId = DB.Annotations.Max(x => x.Id) + 1;
            var annotation = new Annotations
            {
                Id = nextId,
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

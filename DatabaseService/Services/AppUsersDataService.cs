using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Linq;

namespace DatabaseService.Services
{
    public class AppUsersDataService : IAppUsersDataService
    {
        public Annotations CreateAnnotations(Annotations obj)
        {
            using var DB = new StackoverflowContext();
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
            using var DB = new StackoverflowContext();
            var result = DB.Annotations.Find(value);
            return result;
        }

        public bool DeleteAnnotation(int id)
        {
            using var DB = new StackoverflowContext();
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

        public bool CreateAnnotation_withFunction(Annotations obj)
        {
            using var DB = new StackoverflowContext();
            var appUserId = obj.UserId;
            var postId = obj.HistoryId;
            if (string.IsNullOrEmpty(obj.Body)) // need to validate the rest as well somehow :) 
            {
                return false;
            }
            /*var annotationBody = new NpgsqlParameter("body", NpgsqlTypes.NpgsqlDbType.Text); // these 2 guys trigger error from db:column body does not exist but this is false!!
            annotationBody.Value = obj.Body;*/
            var annotationBody = obj.Body; // this is not validated for now because of the above comment! 
            DB.Database.ExecuteSqlCommand($"select * from annotate({appUserId}, {postId}, {annotationBody})"); // need to make this one return something or learn to use the logger bleah! 
            return true;
        }

        public bool UpdateAnnotationBody(Annotations annotationObj)
        {
            Console.WriteLine("LOOK HERE!!!!!!!!!!!: !!!!!!!");
            Console.WriteLine(annotationObj);
            using var DB = new StackoverflowContext();
            try
            {
                var annotationToUpdate = DB.Annotations.Find(annotationObj.Id);
                annotationToUpdate.Body = annotationObj.Body;
                DB.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /* private DateTime UnixTimestamp()
         {

             var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
             return Timestamp;
         }
 */

    }

}

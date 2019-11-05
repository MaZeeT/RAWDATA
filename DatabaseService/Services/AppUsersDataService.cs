using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            var newAnnotation = new Annotations
            {
                Id = nextId,
                UserId = obj.UserId,
                HistoryId = obj.HistoryId,
                Body = obj.Body,
                Date = obj.Date
            };
            DB.Annotations.Add(newAnnotation);
            DB.SaveChanges();
            return GetAnnotation(newAnnotation.Id);
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

        public string CreateAnnotation_withFunction(Annotations obj)
        {
            var newAnnotation = new Annotations
            {
                UserId = obj.UserId,
                HistoryId = obj.HistoryId,
                Body = obj.Body,
                Date = obj.Date
            };
            using var DB = new StackoverflowContext();
            var appUserId = obj.UserId;
            var postId = obj.HistoryId;
            var annotationBody = obj.Body;
            var result = DB.Annotations.FromSqlInterpolated($"select * from annotate(2, 71, 'my note for post 71: this post is very relevant')"); // need to make this one return something or learn to use the logger bleah! 
            return string.Empty;
        }

        /* private DateTime UnixTimestamp()
         {

             var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
             return Timestamp;
         }
 */

    }

}

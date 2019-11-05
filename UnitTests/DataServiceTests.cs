using DatabaseService.Modules;
using DatabaseService.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Xunit;

namespace UnitTests
{
    public class DataServiceTests
    {
        [Fact]
        public void Create_New_Annotation()
        {

            var service = new AppUsersDataService();
            // example of request aspect when coming from API
            var newAnnotation = new Annotations
            {
                UserId = 1,
                HistoryId = 19,
                Body = "monimoniMoNitestmoinitest"
            };

            service.CreateAnnotation_withFunction(newAnnotation);

            /*var annotation = service.CreateAnnotations(newAnnotation);
            Assert.True(annotation.Id > 0);
            Assert.Equal(2, annotation.UserId);
            Assert.Equal(2, annotation.HistoryId);
            Assert.Equal("monimoniMoNitestmoinitest", annotation.Body);

            // cleanup
            service.DeleteAnnotation(annotation.Id);*/
        }

        /*[Fact]
        public void GetAnnotationById()
        {
            var service = new AppUsersDataService();
            var result = service.GetAnnotation(1);
            Assert.Equal(2, result.UserId);
            Assert.Equal(3, result.HistoryId);
            Assert.Equal("my note for post 71: this post is very relevant", result.Body);
        }*/
    }
}

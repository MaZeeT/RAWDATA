using DatabaseService;
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
                UserId = 2,
                HistoryId = 71,
                Body = "Moni is a moni test of moni"
            };

            var result = service.CreateAnnotation_withFunction(newAnnotation);
            Assert.True(result);

            // cleanup
            //service.DeleteAnnotation(annotation.Id);
        }
/*
        [Fact]
        public void SearchTest()
        {
            var service = new DataService();
            var list = service.Search("chocolate");
            Assert.True(list.Count > 0);
        }
*/
        [Fact]
        public void GetAnnotationById()
        {
            var service = new AppUsersDataService();
            var result = service.GetAnnotation(1);
            Assert.Equal(2, result.UserId);
            Assert.Equal(3, result.HistoryId);
            Assert.Equal("my note for post 71: this post is very relevant", result.Body);
        }

        [Fact]
        public void UpdateExistingAnnotation()
        {
            var service = new AppUsersDataService();
            var newAnnotation = new Annotations
            {
                Id = 2,
                Body = "This is updated annotation body becase we can :) ! <3"
            };
            var result = service.UpdateAnnotationBody(newAnnotation);
            Assert.True(result);
        }

    }
}

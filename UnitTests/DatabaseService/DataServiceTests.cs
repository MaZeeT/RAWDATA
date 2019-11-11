using DatabaseService.Modules;
using DatabaseService.Services;
using Xunit;

namespace UnitTests.DatabaseService
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
                Body = "This is a test :) "
            };

            var result = service.CreateAnnotation_withFunction(newAnnotation, out newAnnotation);
            Assert.True(result);
            Assert.True(newAnnotation.Id > 0);

            // cleanup
            service.DeleteAnnotation(newAnnotation.Id);
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

        /*[Fact]
        public void UpdateExistingAnnotation()
        {
            var service = new AppUsersDataService();
            var newAnnotation = new AnnotationsDto
            {
                AnnotationId = 2,
                Body = "This is updated annotation body becase we can :) ! <3"
            };
            var result = service.UpdateAnnotationBody(newAnnotation);
            Assert.True(result);
        }*/

    }
}

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

            var service = new AnnotationService();
            // example of request aspect when coming from API
            var newAnnotation = new AnnotationsDto
            {
                UserId = 2,
                HistoryId = 71,
                Body = "This is a test :) "
            };

            // need to see why this test returns false;
            // should run on a cloud db;
            var result = service.CreateAnnotation_withFunction(newAnnotation, out int newAnnotationId);

            Assert.True(result);
            Assert.True(newAnnotationId > 0);

            var getRes = service.GetAnnotationByUserId(newAnnotationId, 2);
            Assert.Equal("This is a test :) ", getRes.Body);

            var alterBody = "Updated body of this is a test annotation";
            var isUpdated = service.UpdateAnnotation(newAnnotationId, alterBody);
            Assert.True(isUpdated);
            // cleanup
            var delete = service.DeleteAnnotation(newAnnotationId, 2);
            Assert.True(delete);
        }

    }
}

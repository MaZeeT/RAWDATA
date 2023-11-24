using DatabaseService.Modules;
using System.Collections.Generic;

namespace DatabaseService.Services
{
    public interface IAnnotationService
    {
        Annotations CreateAnnotations(AnnotationsDto annotationObject);
        Annotations GetAnnotation(int annotationId);
        Annotations GetAnnotationByUserId(int annotationId, int userId);
        List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId, PagingAttributes pagingAttributes);
        List<PostAnnotationsDto> GetAllAnnotationsOfUser(int userId, PagingAttributes pagingAttributes, out int count);
        bool UpdateAnnotation(int annotationId, string annotationBody);
        bool DeleteAnnotation(int id, int userId);
        bool CreateAnnotation_withFunction(AnnotationsDto newAnnotation, out int newId);
        int GetAllAnnotationsOfUserCount(int userId);
        int UserAnnotOnPostListCount(int userId, int postId);
    }
}

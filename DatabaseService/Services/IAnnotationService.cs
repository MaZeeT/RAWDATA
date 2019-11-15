using DatabaseService.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        bool CreateAnnotation_withFunction(AnnotationsDto newAnnotation, out Annotations annotationFromDb);
        int GetAllAnnotationsOfUserCount(int userId);
        int UserAnnotOnPostListCount(int userId, int postId);
    }
}

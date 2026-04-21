using Domain.Models;

namespace DomainServices.Interfaces;

public interface IAnnotationService
{
    Annotations? GetAnnotation(int annotationId);
    List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId, PagingAttributes pagingAttributes);
    List<PostAnnotationsDto> GetAllAnnotationsOfUser(int userId, PagingAttributes pagingAttributes, out int count);
    bool UpdateAnnotation(int annotationId, string annotationBody);
    bool DeleteAnnotation(int id, int userId);
    bool CreateAnnotation(AnnotationsDto newAnnotation, out int newId);
}

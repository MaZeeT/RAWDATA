using Domain.Models;

namespace Repositories.Interfaces;

public interface IAnnotationRepository
{
    Annotations? GetAnnotation(int annotationId);
    List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId, PagingAttributes pagingAttributes);
    List<PostAnnotationsDto> GetAllAnnotationsOfUser(int userId, PagingAttributes pagingAttributes, out int count);
    bool UpdateAnnotation(int annotationId, string annotationBody);
    bool DeleteAnnotation(int id, int userId);
    bool AddAnnotation(AnnotationsDto newAnnotation, out int newId);
}


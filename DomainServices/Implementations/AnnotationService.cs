using BusinessLogic.Interfaces;
using Domain.AnnotationsDTOs;
using Domain.Services;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

public class AnnotationService : IAnnotationService
{
    private readonly IAnnotationRepository _annotationRepository;

    public AnnotationService(IAnnotationRepository annotationRepository)
    {
        _annotationRepository = annotationRepository;
    }

    public Annotations GetAnnotation(int annotationId)
    {
        return _annotationRepository.GetAnnotation(annotationId);
    }

    public List<SimpleAnnotationDto> GetUserAnnotationsMadeOnAPost(int userId, int postId, PagingAttributes pagingAttributes)
    {
        return _annotationRepository.GetUserAnnotationsMadeOnAPost(userId, postId, pagingAttributes);
    }

    public List<PostAnnotationsDto> GetAllAnnotationsOfUser(int userId, PagingAttributes pagingAttributes, out int count)
    {
        return _annotationRepository.GetAllAnnotationsOfUser(userId, pagingAttributes, out count);
    }

    public bool UpdateAnnotation(int annotationId, string annotationBody)
    {
        return _annotationRepository.UpdateAnnotation(annotationId, annotationBody);
    }

    public bool DeleteAnnotation(int id, int userId)
    {
        return _annotationRepository.DeleteAnnotation(id, userId);
    }

    public bool CreateAnnotation_withFunction(AnnotationsDto newAnnotation, out int newId)
    {
        return _annotationRepository.CreateAnnotation_withFunction(newAnnotation, out newId);
    }
}

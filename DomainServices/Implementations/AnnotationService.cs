using Application.Interfaces;
using Domain.Models;
using DomainServices.Interfaces;

namespace DomainServices.Implementations;

public class AnnotationService : IAnnotationService
{
    private readonly IAnnotationRepository _annotationRepository;
    private readonly IUserRepository _userRepository;

    public AnnotationService(IAnnotationRepository annotationRepository, IUserRepository userRepository)
    {
        _annotationRepository = annotationRepository;
        _userRepository = userRepository;
    }

    public Annotations? GetAnnotation(int annotationId)
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

    public bool CreateAnnotation(AnnotationsDto newAnnotation, out int newId)
    {
        var userExist = _userRepository.AppUserExist(newAnnotation.UserId);
        if (!userExist)
        {
            newId = -1;
            return false;
        }
        
        return _annotationRepository.AddAnnotation(newAnnotation, out newId);
    }
}

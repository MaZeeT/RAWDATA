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
        List<Annotations> GetAllAnnotationsByUserId(int userId);
        /*List<Annotations> GetAnnotationsByPostId(int userId, int postId); // Not used for now -> returns the simple annotation without question and such */
        List<AnnotationsQuestions> GetAnnotationsAndQuestionsByPostId(int userId, int postId);
        bool UpdateAnnotation(int annotationId, string annotationBody);
        bool DeleteAnnotation(int id);
        bool CreateAnnotation_withFunction(Annotations newAnnotation, out Annotations annotationFromDb);
    }
}

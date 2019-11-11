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
        bool UpdateAnnotation(int annotationId, string annotationBody);
        bool DeleteAnnotation(int id);
        bool CreateAnnotation_withFunction(Annotations newAnnotation, out Annotations annotationFromDb);
    }
}

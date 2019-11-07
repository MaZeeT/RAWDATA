using DatabaseService.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseService.Services
{
    public interface IAppUsersDataService
    {
        public Annotations CreateAnnotations(AnnotationsDto annotationObject);

        Annotations GetAnnotation(int annotationId);

        bool CreateAnnotation_withFunction(Annotations annotationObject);
        bool UpdateAnnotation(int annotationId, string annotationBody);
        bool DeleteAnnotation(int id);
    }
}

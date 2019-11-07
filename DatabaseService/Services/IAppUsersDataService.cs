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

        public Annotations GetAnnotation(int annotationId);

        public bool CreateAnnotation_withFunction(Annotations annotationObject);
    }
}

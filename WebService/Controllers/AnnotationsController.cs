using AutoMapper;
using DatabaseService.Modules;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/annotations")]
    public class AnnotationsController : ControllerBase
    {
        private IAppUsersDataService _appUsersDataService;
        private IMapper _mapper;

        public AnnotationsController(IAppUsersDataService appUsersDataService, IMapper mapper)
        {
            _appUsersDataService = appUsersDataService;
            _mapper = mapper;
        }

        [HttpGet("{annotationId}", Name = nameof(ReturnAnnotation))] // fancy way to have strings checked by the compiler
        public ActionResult ReturnAnnotation(int annotationId)
        {
            var returnedAnnotation = _appUsersDataService.GetAnnotation(annotationId);
            if (returnedAnnotation == null)
            {
                return NotFound();
            }
            //with the helper class and the mapper we are setting the annotation type result (in returnAnnotation)
            //to AnnotationDto class type
            //so the magic is not much as it still requires some manual work for mapping. 
            return Ok(CreateLink(returnedAnnotation));

            //this is a new class actually;
            // also if we do it for this object we can do it for all the other categorie. 
            /*var anotation = new AnnotationsDto
            {
                //// parsing the id as part of the url so one can actually navigate to the annotation if needed??? idk ... maybe it is redundant here.. but still good example :) 
                URL = Url.Link(nameof(ReturnAnnotation), new { AnnotationId = returnedAnnotation.Id }),
                AnnotationId = returnedAnnotation.Id,
                UserId = returnedAnnotation.UserId,
                HistoryId = returnedAnnotation.HistoryId,
                Body = returnedAnnotation.Body,
                Date = returnedAnnotation.Date

            };*/

        }
        /// <summary>
        /// This function calls the create anew annotation from db function
        /// PROBLEM: that create new annotation function will not return anything but void.
        /// QUESTION: should we modify it so it returns the actual object created with it's own id since we might use it... maybe... on the frontend? 
        /// </summary>
        /// <param name="annotationObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateCategory(AnnotationsDto annotationObj)
        {
            //OTHER QUESTION THAT FITS FOR THE REST OF THE APIs as well. Mapping the DTO to the Module so that they are separate on the dataservice layer and webservice layer. 
            // maybe this can be done through a helper class in a helper util folder thinghy? 
            var newAnnotation = new Annotations
            {
                UserId = annotationObj.UserId,
                HistoryId = annotationObj.HistoryId,
                Body = annotationObj.Body
            };
            _appUsersDataService.CreateAnnotation_withFunction(newAnnotation);
            return Ok(newAnnotation);
        }

         
        private AnnotationsDto CreateLink(Annotations annotation)
        {
            var annotationDto = _mapper.Map<AnnotationsDto>(annotation);
            annotationDto.AnnotationId = annotation.Id;
            annotationDto.URL = Url.Link(
                    nameof(ReturnAnnotation),
                    new { AnnotationId = annotation.Id });
            return annotationDto;
        }


    }
}

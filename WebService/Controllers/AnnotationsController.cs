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

        [HttpGet("{annotationId}", Name = nameof(GetAnnotation))] // fancy way to have strings checked by the compiler
        public ActionResult GetAnnotation(int annotationId)
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
        }


        /// <summary>
        /// This function calls the create anew annotation from db function
        /// PROBLEM: that create new annotation function will not return anything but void.
        /// QUESTION: should we modify it so it returns the actual object created with it's own id since we might use it... maybe... on the frontend? 
        /// </summary>
        /// <param name="annotationObj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAnnotation(AnnotationsDto annotationObj)
        {
            var newAnnotation = new Annotations
            {
                UserId = annotationObj.UserId,
                HistoryId = annotationObj.HistoryId,
                Body = annotationObj.Body
            };
            _appUsersDataService.CreateAnnotation_withFunction(newAnnotation);
            return Ok(newAnnotation);
        }


        [HttpPut]
        public ActionResult UpdateAnnotation([FromBody] AnnotationsDto annotation)
        {
            if (_appUsersDataService.UpdateAnnotation(annotation))
            {
                return NoContent();
            }
            return BadRequest();
        }


        /// <summary>
        /// DTO Annotations Mapper
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns>AnnotationsDto</returns>
        private AnnotationsDto CreateLink(Annotations annotation)
        {
            var annotationDto = _mapper.Map<AnnotationsDto>(annotation);
            annotationDto.AnnotationId = annotation.Id;
            annotationDto.URL = Url.Link(
                    nameof(GetAnnotation),
                    new { AnnotationId = annotation.Id });
            return annotationDto;
        }


    }
}

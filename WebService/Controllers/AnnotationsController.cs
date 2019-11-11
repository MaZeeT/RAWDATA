using AutoMapper;
using DatabaseService.Modules;
using DatabaseService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/annotations")]
    [Authorize]
    public class AnnotationsController : ControllerBase
    {
        private IAnnotationService _annotationService;
        private IMapper _mapper;

        public AnnotationsController(IAnnotationService annotationService, IMapper mapper)
        {
            _annotationService = annotationService;
            _mapper = mapper;
        }


        /// <summary>
        /// Testing this api in postman: http://localhost:5001/api/annotations/2
        /// </summary>
        /// <param name="annotationId"></param>
        /// <returns></returns>
        [HttpGet("{annotationId}", Name = nameof(GetAnnotation))] // fancy way to have strings checked by the compiler
        public ActionResult GetAnnotation(int annotationId)
        {
            var returnedAnnotation = _annotationService.GetAnnotation(annotationId);
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
        /// Testing with postman:
        ///  in request: POST http://localhost:5001/api/annotations 
        ///  in body: 
        ///         {
        ///             "UserId": 1,
        ///             "HistoryId": 19,
        ///             "Body": "This call takes in userId, HistoryId and the body; but returns all the things from AnnotationsDto"
        ///         }
        /// </summary>
        /// <param name="annotationObj"></param>
        /// <returns>Returns the </returns>
        [HttpPost]
        public ActionResult AddAnnotation(AnnotationsDto annotationObj)
        {
            var newAnnotation = new Annotations
            {
                UserId = annotationObj.UserId,
                HistoryId = annotationObj.HistoryId,
                Body = annotationObj.Body
            };
            if (_annotationService.CreateAnnotation_withFunction(newAnnotation, out newAnnotation))
            {
                return Ok(CreateLink(newAnnotation));
            }
            return BadRequest();

        }

        /// <summary>
        /// API that needs id of annotation in the request path and body text in body of request 
        /// Testing with postman:
        ///                     request path: http://localhost:5001/api/annotations/52
        ///                     request body:
        ///                     {
        ///                     "Body": "he body of the PUT request can have all annotationsDto values, but it will only update the .Body value"
        ///                     }
        /// </summary>
        /// <param name="annotationId"></param>
        /// <param name="annotation"></param>
        /// <returns>
        ///         Success: 204 NoContent
        ///         Fail: 404 BadRequest
        /// </returns>
        [HttpPut("{annotationId}")]
        public ActionResult UpdateAnnotation(int annotationId,[FromBody] AnnotationsDto annotation)
        {
            //need to encode body before sending to db - this can also be done inside the UpdateAnnotation function.
            if (_annotationService.UpdateAnnotation(annotationId, annotation.Body))
            {
                return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// Delete annotation by providing the id;
        /// Testing with postman: DELETE http://localhost:5001/api/annotations/52
        /// </summary>
        /// <param name="annotationId"></param>
        /// <returns> Success: 200 Ok ; Fail 404 Not Found</returns>
        [HttpDelete("{annotationId}")]
        public ActionResult DeleteData(int annotationId)
        {
            if (_annotationService.DeleteAnnotation(annotationId))
            {
                return Ok();
            }
            return NotFound();
        }



        /// <summary>
        /// DTO Annotations Mapper used with GET annotation
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

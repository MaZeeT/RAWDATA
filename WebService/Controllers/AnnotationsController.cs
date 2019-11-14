using AutoMapper;
using DatabaseService;
using DatabaseService.Modules;
using DatabaseService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/annotations")]
    [Authorize]
    public class AnnotationsController : ControllerBase
    {
        private IAnnotationService _annotationService;
        private IMapper _mapper;
        private ISharedService _sharedService;

        public AnnotationsController(IAnnotationService annotationService, ISharedService sharedService, IMapper mapper)
        {
            _annotationService = annotationService;
            _sharedService = sharedService;
            _mapper = mapper;
        }


        /// <summary>
        /// Get all annotations that belong to the logged in user : http://localhost:5001/api/annotations plus Authorization Bearer <valid_tokenvalue> in Headers
        /// </summary>
        /// <returns>Array of Json Annotation Objects plus relevant status code</returns>
        [HttpGet("post/{postId}")]
        public ActionResult GetAllUserAnnotationsMadeOnPostId(int postId, [FromQuery] PagingAttributes pagingAttributes) //needs-pagination
        {
            int userIdFromToken = GetAuthUserId();
            var listOfAnnotations = _annotationService.GetUserAnnotationsMadeOnAPost(userIdFromToken, postId, pagingAttributes);
            if (listOfAnnotations.Count == 0)
            {
                return NotFound();
            }
            
            return Ok(listOfAnnotations);
        }
        /// <summary>
        /// Get all annoations made by a user on a post based on postid and userid
        /// based on postId/ questionId and not historyId from url and userId from token
        /// http://localhost:5001/api/annotations/user/{questionId}
        /// header: valid user token;
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>List of annotationsDto</returns>
        [HttpGet("user/{postId}")]
        public ActionResult GetAllAnnotationsByPostId(int postId, [FromQuery] PagingAttributes pagingAttributes)
        {
            int userIdFromToken = GetAuthUserId();
            var listOfAnnotations = _annotationService.GetAllAnnotationsOfUser(userIdFromToken, postId, pagingAttributes);
            if (listOfAnnotations.Count == 0)
            {
                return NotFound();
            }
            foreach (PostAnnotationsDto item in listOfAnnotations)
            {
                var postDataForAnnot = _sharedService.GetPost(item.PostId);
                item.PostId = postDataForAnnot.Id;
                item.QuestionId = postDataForAnnot.QuestionId;
                item.Title = postDataForAnnot.Title;
                item.PostBody = postDataForAnnot.Body;
            }

            return Ok(listOfAnnotations);

        }

        /// <summary>
        /// Testing this api in postman: http://localhost:5001/api/annotations/2
        /// </summary>
        /// <param name="annotationId"></param>
        /// <returns>AnnotationDto</returns>
        [HttpGet("{annotationId}", Name = nameof(GetAnnotationById))] // fancy way to have strings checked by the compiler
        public ActionResult GetAnnotationById(int annotationId)
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
        ///  in request: POST http://localhost:5001/api/annotations  plus valid token of the user
        ///  in body: 
        ///         {
        ///             "HistoryId": 19, //fix namig because it is postid
        ///             "Body": "This call takes in userId, HistoryId and the body; but returns all the things from AnnotationsDto"
        ///         }
        /// </summary>
        /// <param name="annotationObj"></param>
        /// <returns>Returns the </returns>
        [HttpPost (Name = nameof(AddAnnotation))]
        public ActionResult AddAnnotation(AnnotationsDto annotationObj)
        {
            var newAnnotation = new Annotations
            {
                UserId = GetAuthUserId(),
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
                    nameof(GetAnnotationById),
                    new { AnnotationId = annotation.Id });
            annotationDto.AddAnnotationUrl = Url.ActionLink(nameof(AddAnnotation));
            return annotationDto;
        }
   

        /// <summary>
        /// Get the authenticated user's id from token claim
        /// </summary>
        /// <returns>integer authenticated user's id from token</returns>
        private int GetAuthUserId()
        {
            var userIdFromToken = int.Parse(this.User.Identity.Name);
            return userIdFromToken;
        }


    }
}

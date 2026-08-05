using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Entities;
using Web.Extensions;

namespace Web.Controllers;

[ApiController]
[Route("api/annotations")]
[Authorize]
public class AnnotationsController : ControllerBase
{
    private readonly IAnnotationService _annotationService;
    private readonly IThreadService _threadService;

    public AnnotationsController(IAnnotationService annotationService, IThreadService threadService)
    {
        _annotationService = annotationService;
        _threadService = threadService;
    }
    
    [HttpGet("post/{postId:int}")]
    public ActionResult GetAllUserAnnotationsMadeOnPostId(int postId, [FromQuery] PagingAttributes pagingAttributes) //needs-pagination
    {
        var userIdResult = User.GetUserId();
        
        if (!userIdResult.IsSuccess)
        {
            return Unauthorized();
        }

        var listOfAnnotations = _annotationService.GetUserAnnotationsMadeOnAPost(userIdResult.Value, postId, pagingAttributes);
        
        if (listOfAnnotations.Count == 0)
        {
            return NotFound();
        }

        return Ok(listOfAnnotations);
    }

    [HttpGet("user", Name = nameof(GetAllAnnotationsOfUser))]
    public ActionResult GetAllAnnotationsOfUser([FromQuery] PagingAttributes pagingAttributes)
    {
        var userIdResult = User.GetUserId();
        
        if (!userIdResult.IsSuccess)
        {
            return Unauthorized();
        }

        var listOfAnnotations = _annotationService.GetAllAnnotationsOfUser(userIdResult.Value, pagingAttributes, out var count);
        if (count == 0)
        {
            return NotFound();
        }

        foreach (var annotation in listOfAnnotations)
        {
            if (annotation.PostId == null) 
                continue;
            
            var postDataForAnnot = _threadService.GetPost(annotation.PostId.Value);
            annotation.PostId = postDataForAnnot.Id;
            annotation.QuestionId = postDataForAnnot.QuestionId;
            annotation.Title = postDataForAnnot.Title;
            annotation.PostBody = postDataForAnnot.Body;
            annotation.PostUrl = SetPostUrl(postDataForAnnot.Id, postDataForAnnot.QuestionId);
        }

        var result = CreateResult(listOfAnnotations, pagingAttributes, count); 
        return Ok(result);
    }

    [HttpGet("{annotationId:int}",
        Name = nameof(GetAnyAnnotationById))] // fancy way to have strings checked by the compiler
    public ActionResult GetAnyAnnotationById(int annotationId)
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

    [HttpPost(Name = nameof(AddAnnotation))]
    public ActionResult AddAnnotation([FromBody] AnnotationsDto annotationObj)
    {
        var userIdResult = User.GetUserId();
        
        if (!userIdResult.IsSuccess)
        {
            return Unauthorized();
        }

        var newAnnotation = new AnnotationsDto
        {
            UserId = userIdResult.Value,
            PostId = annotationObj.PostId,
            Body = annotationObj.Body
        };

        if (_annotationService.CreateAnnotation(newAnnotation, out var newId))
        {
            var createdAnnotation = _annotationService.GetAnnotation(newId);
            return Ok(CreateLink(createdAnnotation));
        }

        return BadRequest();
    }

    [HttpPut("{annotationId:int}")]
    public ActionResult UpdateAnnotation(int annotationId, [FromBody] AnnotationsDto annotation)
    {
        //need to encode body before sending to db - this can also be done inside the UpdateAnnotation function.
        if (_annotationService.UpdateAnnotation(annotationId, annotation.Body))
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpDelete("{annotationId:int}")]
    public ActionResult DeleteAnnotation(int annotationId)
    {
        var userIdResult = User.GetUserId();
        
        if (!userIdResult.IsSuccess)
        {
            return Unauthorized();
        }

        if (_annotationService.DeleteAnnotation(annotationId, userIdResult.Value))
        {
            return Ok();
        }

        return NotFound();
    }

    private AnnotationsDto CreateLink(Annotations annotation)
    {
        var annotationDto = AnnotationsDto.MapFrom(annotation);
        annotationDto.AnnotationId = annotation.Id;
        annotationDto.URL = Url.Link(
            nameof(GetAnyAnnotationById),
            new { AnnotationId = annotation.Id });
        annotationDto.AddAnnotationUrl = Url.ActionLink(nameof(AddAnnotation));
        return annotationDto;
    }

    private string SetPostUrl(int pId, int qId)
    {
        var urlString = Url.Link(nameof(QuestionsController.GetThread), new { questionId = qId, postId = pId });
        return urlString;
    }

    private object CreateResult(List<PostAnnotationsDto> itemList, PagingAttributes attr, int totalItems)
    {
        var numberOfPages = Math.Ceiling((double)totalItems / attr.PageSize);

        var prev = attr.Page > 1
            ? Url.Link(nameof(GetAllAnnotationsOfUser), new { page = attr.Page - 1, pageSize = attr.PageSize })
            : null;
        var next = attr.Page < numberOfPages
            ? Url.Link(nameof(GetAllAnnotationsOfUser), new { page = attr.Page + 1, pageSize = attr.PageSize })
            : null;

        return new
        {
            totalItems,
            numberOfPages,
            prev,
            next,
            items = itemList
        };
    }
}
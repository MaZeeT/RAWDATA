using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Domain;
using Domain.AnnotationsDTOs;
using Domain.Services;
using Repositories.Interfaces;

namespace WebService.Controllers;

[ApiController]
[Route("api/annotations")]
[Authorize]
public class AnnotationsController : SharedController
{
    private IAnnotationRepository _annotationRepositoryService;
    private ISharedRepository _sharedRepositoryService;

    public AnnotationsController(IAnnotationRepository annotationRepositoryService, ISharedRepository sharedRepositoryService)
    {
        _annotationRepositoryService = annotationRepositoryService;
        _sharedRepositoryService = sharedRepositoryService;
    }

    [HttpGet("post/{postId}")]
    public ActionResult
        GetAllUserAnnotationsMadeOnPostId(int postId,
            [FromQuery] PagingAttributes pagingAttributes) //needs-pagination
    {
        (int userId, bool useridok) = GetAuthUserId();
        if (!useridok){ return Unauthorized(); }

        var listOfAnnotations = _annotationRepositoryService.GetUserAnnotationsMadeOnAPost(userId, postId, pagingAttributes);
        if (listOfAnnotations.Count == 0){ return NotFound(); }

        return Ok(listOfAnnotations);
    }

    [HttpGet("user", Name = nameof(GetAllAnnotationsOfUser))]
    public ActionResult GetAllAnnotationsOfUser([FromQuery] PagingAttributes pagingAttributes)
    {
        (int userId, bool useridok) = GetAuthUserId();
        if (!useridok)
        {
            return Unauthorized();
        }

        int count;
        var listOfAnnotations = _annotationRepositoryService.GetAllAnnotationsOfUser(userId, pagingAttributes, out count);
        if (count == 0)
        {
            return NotFound();
        }

        foreach (PostAnnotationsDto item in listOfAnnotations)
        {
            var postDataForAnnot = _sharedRepositoryService.GetPost(item.PostId);
            item.PostId = postDataForAnnot.Id;
            item.QuestionId = postDataForAnnot.QuestionId;
            item.Title = postDataForAnnot.Title;
            item.PostBody = postDataForAnnot.Body;
            item.PostUrl = SetPostUrl(postDataForAnnot.Id, postDataForAnnot.QuestionId);
        }

        return Ok(CreateResult(listOfAnnotations, pagingAttributes, count));
    }

    [HttpGet("{annotationId}",
        Name = nameof(GetAnyAnnotationById))] // fancy way to have strings checked by the compiler
    public ActionResult GetAnyAnnotationById(int annotationId)
    {
        var returnedAnnotation = _annotationRepositoryService.GetAnnotation(annotationId);
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
    public ActionResult AddAnnotation(AnnotationsDto annotationObj)
    {
        (int userId, bool useridok) = GetAuthUserId();
        if (!useridok)
        {
            return Unauthorized();
        }

        var newAnnotation = new AnnotationsDto
        {
            UserId = userId,
            PostId = annotationObj.PostId,
            Body = annotationObj.Body
        };
        int newId;
        if (_annotationRepositoryService.CreateAnnotation_withFunction(newAnnotation, out newId))
        {
            var createdAnnotation = _annotationRepositoryService.GetAnnotation(newId);
            return Ok(CreateLink(createdAnnotation));
        }

        return BadRequest();
    }

    [HttpPut("{annotationId}")]
    public ActionResult UpdateAnnotation(int annotationId, [FromBody] AnnotationsDto annotation)
    {
        //need to encode body before sending to db - this can also be done inside the UpdateAnnotation function.
        if (_annotationRepositoryService.UpdateAnnotation(annotationId, annotation.Body))
        {
            return NoContent();
        }

        return BadRequest();
    }

    [HttpDelete("{annotationId}")]
    public ActionResult DeleteAnnotation(int annotationId)
    {
        (int userId, bool useridok) = GetAuthUserId();
        if (!useridok)
        {
            return Unauthorized();
        }

        if (_annotationRepositoryService.DeleteAnnotation(annotationId, userId))
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
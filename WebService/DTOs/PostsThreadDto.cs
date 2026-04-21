using System.Collections.Generic;
using Domain.Entities;

namespace WebService.DTOs;

public class PostsThreadDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int ParentId { get; set; }
    public List<SimpleAnnotationDto> Annotations { get; set; }
    public string CreateAnnotationLink { get; set; }
    public string CreateBookmarkLink { get; set; }

}
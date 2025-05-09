using System.Collections.Generic;
using Domain.AnnotationsDTOs;

namespace WebService.DTOs;

public class PostsThreadDto
{
    public int Id { get; internal set; }
    public string Title { get; internal set; }
    public string Body { get; internal set; }
    public int Parentid { get; internal set; }
    public List<SimpleAnnotationDto> Annotations { get; set; }
    public string createAnnotationLink { get; set; }
    public string createBookmarkLink { get; set; }

}
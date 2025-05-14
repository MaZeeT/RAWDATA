using Domain.AnnotationsDTOs;

namespace WebDTOs;

public class PostsThreadDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int Parentid { get; set; }
    public List<SimpleAnnotationDto> Annotations { get; set; }
    public string createAnnotationLink { get; set; }
    public string createBookmarkLink { get; set; }

}
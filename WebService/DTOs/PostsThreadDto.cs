namespace WebService
{
    public class PostsThreadDto
    {
        public int Id { get; internal set; }
        public string Title { get; internal set; }
        public string Body { get; internal set; }
        public int Parentid { get; internal set; }
        public string createAnnotationLink { get; set; }
        public string createBookmarkLink { get; set; }

    }

}
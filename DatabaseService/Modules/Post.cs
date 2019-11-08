namespace DatabaseService
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Parentid { get; set; }
        public string Body { get; set; }
    }
}
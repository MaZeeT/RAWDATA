namespace WebService
{
    public class PostsSearchList
    {
        public string ThreadLink { get; set; }
        public decimal Rank { get; set; }
        public int PostId { get; internal set; }
        public string QuestionTitle { get; internal set; }
        public string PostBody { get; internal set; }

    }

}
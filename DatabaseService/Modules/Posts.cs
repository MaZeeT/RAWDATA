namespace DatabaseService
{
    public class Posts
    {
        public int Id { get; internal set; }
        public decimal Rank { get; set; }
        public string Title { get; internal set; }
        public string Body { get; internal set; }
        public int Parentid { get; internal set; }
        public int Totalresults { get; internal set; }

    }

}
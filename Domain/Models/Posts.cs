namespace Domain.Models
{
    public class Posts
    {
        public int Id { get; set; }
        public decimal Rank { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Parentid { get; set; }
        public int Totalresults { get; set; }
        
}

}
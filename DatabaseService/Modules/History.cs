namespace DatabaseService.Modules
{
    public class History
    {
        public int Id { get; }
        public int Userid { set; get; }
        public int Postid { set; get; }
        public string PostsTableName { set; get; }
        public string Date { set; get; }
        public bool isBookmark { set; get; }
        
    }
}
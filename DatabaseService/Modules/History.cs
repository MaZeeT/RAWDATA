namespace DatabaseService.Modules
{
    public class History
    {
        public int Id { get; set; }
        public int Userid { get; set; }
        public int Postid { get; set; }
        public string PostsTableName { get; set; }
        public string Date { get; set; }
        public bool isBookmark { get; set; }

    }
}
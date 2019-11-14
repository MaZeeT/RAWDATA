using System;

namespace DatabaseService.Modules
{
    public class History
    {
        public int Id { set; get; }
        public int Userid { set; get; }
        public int Postid { set; get; }
        public string PostTableName { set; get; }
        public DateTime Date { set; get; }
        public bool isBookmark { set; get; }
    }
}
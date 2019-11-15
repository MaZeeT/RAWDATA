using System;

namespace DatabaseService
{
    public class Searches
    {
        public int Id { set; get; }
        public int UserId { set; get; }
        public string SearchType { set; get; }
        public string SearchString { set; get; }
        public DateTime Date { set; get; }
    }
}
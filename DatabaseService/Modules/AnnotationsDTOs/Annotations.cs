using System;

namespace DatabaseService.Modules
{
    public class Annotations
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HistoryId { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }
}

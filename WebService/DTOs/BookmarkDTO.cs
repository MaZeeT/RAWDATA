using System;

namespace WebService.DTOs
{
    public class BookmarkDTO
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string ThreadUrl { get; set; }
        public DateTime Date { get; set; }
        public int PostId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseService.Modules
{
    public class AnnotationsQuestions
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string PostTableName { get; set; }
        public DateTime AnnotationCreationDate { get; set; }
        public bool IsBookmark { get; set; }
        public int HistPostId { get; set; }
        public int QuestionId { get; set; }
        public int OwnerId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionBody { get; set; }
        public int AcceptedAnswerId { get; set; }
        public DateTime QuestionCreationDate { get; set; }
        public DateTime QuestionClosedDate { get; set; }
        public int Score { get; set; }
    }
}

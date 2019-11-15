using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseService.Modules
{
    /// <summary>
    /// Deals with the information of annotations that exist on a post (specified by postid)
    /// Could have used AnnotationsDto but preffered to not return empty values where not filled
    /// </summary>
    public class PostAnnotationsDto
    {
        public int AnnotationId { get; set; } 
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public int PostId { get; set; }
        public int QuestionId { get; set; }
        public string Title { get; set; }
        public string PostBody { get; set; }
        public string PostUrl { get; set; }

    }
}

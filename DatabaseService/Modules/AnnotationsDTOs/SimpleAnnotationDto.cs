using System;

namespace DatabaseService.Modules
{
    public class SimpleAnnotationDto
    {
        public int AnnotationId { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }
}

﻿using System;

namespace DatabaseService.Modules
{
    public class AnnotationsDto
    {
        public string URL { get; set; }
        public int AnnotationId { get; set; } // hide from user
        public int UserId { get; set; }  // hide from user
        public int HistoryId { get; set; }  // hide from user
        public int PostId { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; } 
        public string AddAnnotationUrl { get; set; }
    }
}

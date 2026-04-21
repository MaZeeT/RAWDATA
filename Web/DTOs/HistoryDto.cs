using System;

namespace Web.DTOs;

public class HistoryDto
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string ThreadUrl { get; set; }
    public DateTime Date { get; set; }
}
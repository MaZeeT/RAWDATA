using System;

namespace WebService.DTOs;

public class SearchHistoryListDto
{
    public string SearchLink { get; set; }

    public string SearchMethod { get; set; }
    public string SearchString { get; set; }
    public DateTime Date { set; get; }

}
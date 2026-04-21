using System;
using Domain.Enums;

namespace Web.DTOs;

public class SearchHistoryListDto
{
    public string SearchLink { get; set; }

    public SearchType SearchMethod { get; set; }
    public string SearchString { get; set; }
    public DateTime Date { set; get; }

}
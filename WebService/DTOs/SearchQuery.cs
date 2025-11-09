using Microsoft.AspNetCore.Mvc;

namespace WebService.DTOs;

public class SearchQuery
{
    [FromQuery(Name = "s")] //Todo remove this mapping from SearchTerm to s when frontend is being worked on
    public string SearchTerms { get; set; } // comma-delimited
    
    [FromQuery(Name = "stype")] //Todo remove this mapping from SearchType to stype when frontend is being worked on
    public int SearchType { get; set; } = 3; // this sets stype to 3 if there is no stype param
}
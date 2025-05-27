namespace WebService.DTOs;

public class SearchQuery
{
    public string SearchTerms { get; set; } // comma-delimited
    public int SearchType { get; set; } = 3; // this sets stype to 3 if there is no stype param
}
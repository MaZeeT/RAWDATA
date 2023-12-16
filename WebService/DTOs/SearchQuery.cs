namespace DatabaseService;

public class SearchQuery
{
    //searchquery parameters contain 1 string with search terms, comma-delimited & search type
    public string s { get; set; }
    public int stype { get; set; } = 3; //this sets stype to 3 if there is no stype param
}

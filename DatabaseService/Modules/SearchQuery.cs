namespace DatabaseService
{
    public class SearchQuery
    {

        //searchquery parameters contain 1 string with search terms

        // 1 optional searchtype (default searchtype is appsearch bestmatch)
        public string s { get; set; }
        public int stype { get; set; } = 3;
    }
}

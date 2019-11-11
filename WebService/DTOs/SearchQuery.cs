namespace DatabaseService
{
    public class SearchQuery
    {

        //searchquery parameters contain 1 string with search terms

        // 1 optional searchtype (default searchtype is appsearch bestmatch)
        //public const int defstype = 3;
        //private int _stype = defstype;
        //I DONT SEE how to have a param BOTH optional and with default val, which bugs me a lot
        public string s { get; set; }
        public int stype { get; set; } = 3; //this sets stype to 3 if there is no stype param
    }
}

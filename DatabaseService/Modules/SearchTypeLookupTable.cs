using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseService.Modules
{
    class SearchTypeLookupTable
    {
        public string[] searchType = new string[6];
        public SearchTypeLookupTable()
        {
            searchType[0] = "tfidf";
            searchType[1] = "exactmatch";
            searchType[2] = "simple";
            searchType[3] = "bestmatch"; //not currently used
            searchType[4] = "wordstfidf";
            searchType[5] = "wordsbest";
        }
    }
}

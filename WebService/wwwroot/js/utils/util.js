define([""], function () {

    //return named querystring value
    function getParameterByName(name, url) {

        //example url: http://localhost:5001/api/search?s=gnu,bear,gcc&stype=0&page=1&pageSize=5
        //example name: page

        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'), //this line builds the regex string
        //regex now contains the following /[?&]page(=([^&#]*)|&|#|$)/ - this matches from the ? or & character before 'page' to the & # $ after
            results = regex.exec(url);
        //regex.exec() returns an array of matches
        //results now contain ["?page=2", "=2", "2"]

        if (!results) return null; //if nothing matched at all
        if (!results[2]) return ''; //if there was no value (only '=')
        return decodeURIComponent(results[2]); //transform special characters (if any) to original form (like %20 becomes ' ') and return the queryvalue we have now located
    };


    ////Function that builds the URL - could be taken out in a utils file/ folder and used wherever in the code needed. 
    function conputeUrlStringWithPagination(searchStr, searchTypeVal, pageItemSize, pageNo) {
        const searchString = searchStr ? "?s=" + searchStr : "";
        const searchType = searchTypeVal ? "&stype=" + searchTypeVal : "&stype=0";
        const pageSize = pageItemSize ? "&pageSize=" + pageItemSize : "&pageSize=5";
        const pageNumber = pageNo ? "&page=" + pageNo : "&page=1";

        let paginationObject = {
            searchString,
            searchType,
            pageSize,
            pageNumber
        }
        return paginationObject;
    }

    function searchTypeSelectorMapping(value) {
        switch (value) {
            case "TFIDF":
                return 0;
                break;
            case "Exact Match":
                return 1;
                break;
            case "Simple Match":
                return 2;
                break;
            case "Best Match":
                return 3;
                break;
            default:
                return 0;
        }
    }

    return {
        conputeUrlStringWithPagination,
        searchTypeSelectorMapping,
        getParameterByName
    }
})

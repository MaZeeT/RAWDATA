define([""], function () {

    //return named querystring value
    function getParameterByName(name, url) {
        //if (!url) url = window.location.href; //if no url was passed, try to get the current one, we dont use this in this app
        //console.log("namfore: ", name);
        //name = name.replace(/[\[\]]/g, '\\$&');
        //console.log("namfore: ", name);

        //example url: http://localhost:5001/api/search?s=gnu,bear,gcc&stype=0&page=1&pageSize=5
        //example name: page

        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'), //this line builds the regex string
        //regex now contain the following /[?&]page(=([^&#]*)|&|#|$)/
            results = regex.exec(url);
        //regex.exec() return an array of matches
        //results now contain ["?page=2", "=2", "2"]
        console.log("regex: ", regex);
        console.log("resul: ", results);
        console.log("resul0: ", results[0]);
        console.log("resul1: ", results[1]);
        console.log("resul2: ", results[2]);
        if (!results) return null; //if nothing matched at all
        if (!results[2]) return ''; //if there was no value (only '=')
        return decodeURIComponent(results[2]); //return special characters (if any) to original form (like %20 - ' ')
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

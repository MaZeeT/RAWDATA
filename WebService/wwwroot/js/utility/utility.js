define([""], function () {
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
    
    function whatDoWeSay(message){
        console.log(message)
    }

    return {
        conputeUrlStringWithPagination,
        searchTypeSelectorMapping,
        whatDoWeSay
    }
});

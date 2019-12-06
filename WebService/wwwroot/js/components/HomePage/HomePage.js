define(["knockout", "homeService"], function (ko, homeserv) {

    return function () {


        let searchResultDS = ko.observableArray();
        var firstNames = [{ "name": "David" }, { "name": "Drew" }, { "name": "Preston" }, { "name": "Neville" }, { "name": "Garrett" }, { "name": "Jerry" }, { "name": "George" }, { "name": "Brendan" }, { "name": "Xenos" }, { "name": "Camden" }, { "name": "Travis" }, { "name": "Graiden" }, { "name": "Kieran" }, { "name": "Drew" }, { "name": "Hiram" }, { "name": "Abel" }, { "name": "Drew" }, { "name": "Herman" }, { "name": "Ryan" }, { "name": "Garrett" }, { "name": "Melvin" }, { "name": "Jeremy" }, { "name": "Harrison" }, { "name": "Salvador" }, { "name": "Malcolm" }, { "name": "Slade" }, { "name": "Davis" }, { "name": "Bradley" }, { "name": "Curran" }, { "name": "Grady" }, { "name": "Cameron" }, { "name": "Mark" }, { "name": "Amery" }, { "name": "Jamal" }, { "name": "Kyle" }, { "name": "Salvador" }, { "name": "Colby" }, { "name": "Rigel" }, { "name": "Ashton" }, { "name": "Gregory" }, { "name": "Chancellor" }, { "name": "Emerson" }, { "name": "Cruz" }, { "name": "Magee" }, { "name": "Edward" }, { "name": "Nero" }, { "name": "Roth" }, { "name": "Merrill" }, { "name": "Nathan" }, { "name": "Alfonso" }, { "name": "Porter" }, { "name": "Griffith" }, { "name": "Harrison" }, { "name": "Graham" }, { "name": "Dustin" }, { "name": "Orlando" }, { "name": "Tyrone" }, { "name": "Wyatt" }, { "name": "Preston" }, { "name": "Xander" }, { "name": "Phelan" }, { "name": "Brody" }, { "name": "Drew" }, { "name": "Xavier" }, { "name": "Emery" }, { "name": "Kelly" }, { "name": "Hiram" }, { "name": "Rajah" }, { "name": "Judah" }, { "name": "Hunter" }, { "name": "Yoshio" }, { "name": "Ira" }, { "name": "Porter" }, { "name": "Allistair" }, { "name": "Raphael" }, { "name": "Kamal" }, { "name": "Gray" }, { "name": "Daquan" }, { "name": "Ezekiel" }, { "name": "Kevin" }, { "name": "Igor" }, { "name": "Oliver" }, { "name": "Buckminster" }, { "name": "Chaney" }, { "name": "Baker" }, { "name": "Barry" }, { "name": "Macaulay" }, { "name": "Forrest" }, { "name": "Levi" }, { "name": "Jack" }, { "name": "Judah" }, { "name": "Gannon" }, { "name": "Rajah" }, { "name": "Wyatt" }, { "name": "Sebastian" }, { "name": "Anthony" }, { "name": "Tad" }, { "name": "Burton" }, { "name": "Garth" }, { "name": "Dale" }];

        const placeholderStr = "Search with terms here..."
        let searchTerms = ko.observable(placeholderStr);

        let searchResult = ko.observableArray([]);
        let showTable = ko.observable(false);
        let totalResults = ko.observable("0");

        searchTerms.subscribe(function (searchStr) {
            if (searchStr.length === 0) {
                searchResult([]);
                return;
            }
            console.log("Data from user client: ", searchStr);
           
            homeserv.getSearchItems(searchStr, function (responseData) {
                console.log("Data from api call search : ", responseData);
                if (responseData) {
                    totalResults(responseData.totalResults);
                    console.log("Tot res", totalResults);
                    searchResult(responseData.items);
                    showTable(true);
                }
               
            });
        });








        return {
            searchTerms,
            searchResult,
            showTable,
            totalResults

        }
    }

});
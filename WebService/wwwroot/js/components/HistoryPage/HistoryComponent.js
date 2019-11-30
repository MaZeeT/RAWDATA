define(["knockout", "dataService"], function (ko, ds) {

    return function () {
        var search = ko.observable("");

        var firstNames = [
            { "name": "David" }, { "name": "Drew" }, { "name": "Preston" }, { "name": "Neville" },
            { "name": "Garrett" }, { "name": "Jerry" }, { "name": "George" }, { "name": "Brendan" },
            { "name": "Xenos" }, { "name": "Camden" }, { "name": "Travis" }, { "name": "Graiden" },
            { "name": "Kieran" }, { "name": "Drew" }, { "name": "Hiram" }, { "name": "Abel" }, { "name": "Drew" },
            { "name": "Herman" }, { "name": "Ryan" }, { "name": "Garrett" }, { "name": "Melvin" }, { "name": "Jeremy" },
            { "name": "Harrison" }, { "name": "Salvador" }, { "name": "Malcolm" }, { "name": "Slade" },
            { "name": "Davis" }, { "name": "Bradley" }, { "name": "Curran" }, { "name": "Grady" },
            { "name": "Cameron" }, { "name": "Mark" }, { "name": "Amery" }, { "name": "Jamal" }, { "name": "Kyle" },
            { "name": "Salvador" }, { "name": "Colby" }, { "name": "Rigel" }, { "name": "Ashton" },
            { "name": "Gregory" }, { "name": "Chancellor" }, { "name": "Emerson" }, { "name": "Cruz" },
            { "name": "Magee" }, { "name": "Edward" }, { "name": "Nero" }, { "name": "Roth" }, { "name": "Merrill" },
            { "name": "Nathan" }, { "name": "Alfonso" }, { "name": "Porter" }, { "name": "Griffith" },
            { "name": "Harrison" }, { "name": "Graham" }, { "name": "Dustin" }, { "name": "Orlando" },
            { "name": "Tyrone" }, { "name": "Wyatt" }, { "name": "Preston" }, { "name": "Xander" },
            { "name": "Phelan" }, { "name": "Brody" }, { "name": "Drew" }, { "name": "Xavier" }, { "name": "Emery" },
            { "name": "Kelly" }, { "name": "Hiram" }, { "name": "Rajah" }, { "name": "Judah" }, { "name": "Hunter" },
            { "name": "Yoshio" }, { "name": "Ira" }, { "name": "Porter" }, { "name": "Allistair" },
            { "name": "Raphael" }, { "name": "Kamal" }, { "name": "Gray" }, { "name": "Daquan" }, { "name": "Ezekiel" },
            { "name": "Kevin" }, { "name": "Igor" }, { "name": "Oliver" }, { "name": "Buckminster" },
            { "name": "Chaney" }, { "name": "Baker" }, { "name": "Barry" }, { "name": "Macaulay" },
            { "name": "Forrest" }, { "name": "Levi" }, { "name": "Jack" }, { "name": "Judah" }, { "name": "Gannon" },
            { "name": "Rajah" }, { "name": "Wyatt" }, { "name": "Sebastian" }, { "name": "Anthony" }, { "name": "Tad" },
            { "name": "Burton" }, { "name": "Garth" }, { "name": "Dale" }
        ];

        var searchResult = ko.observableArray([]);

        search.subscribe(function (data) {
            if (data.length === 0) {
                searchResult([]);
                return;
            }
            var res = firstNames.filter(x => x.name.toLowerCase().startsWith(data));
            searchResult(res);
        });



        return {
            search,
            firstNames,
            searchResult
        }



    }


});





//function HistoryViewModel() {
//    var self = this;

//    self.history = ko.observableArray([]);

//    self.getHistory = function() {
//        history.push({title: "tester1", url: "www.something.com", date: "today"});
//        history.push({title: "tester32", url: "www.somethingelse.com", date: "22/5"});
//        history.push({title: "tester45", url: "www.goggles.com", date: "24/12"});

//    };

//    self.deleteHistory = function(){
//        history = ko.observableArray([]);
//    };

//}


//ko.applyBindings(new HistoryViewModel);
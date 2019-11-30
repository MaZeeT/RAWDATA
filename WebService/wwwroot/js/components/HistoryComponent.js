

function HistoryViewModel() {
    var self = this;

    self.history = ko.observableArray([]);

    self.getHistory = function() {
        history.push({title: "tester1", url: "www.something.com", date: "today"});
        history.push({title: "tester32", url: "www.somethingelse.com", date: "22/5"});
        history.push({title: "tester45", url: "www.goggles.com", date: "24/12"});

    };

    self.deleteHistory = function(){
        history = ko.observableArray([]);
    };

}


ko.applyBindings(new HistoryViewModel);
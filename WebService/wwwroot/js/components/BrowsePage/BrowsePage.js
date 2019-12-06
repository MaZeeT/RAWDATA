define(['knockout', 'services/BrowseService'], function (ko, bs) {

   


    return function () {
        /*     function QItem(link, id, title, body) {
                 var self = this;
                 self.link = ko.observable(link);
                 self.id = ko.observable(id);
                 self.title = ko.observable(title);
                 self.body = ko.observable(body);
             }
     
             //single year containing an Observable Array of Months (that contain Observable data)
             function QList(totalItems, numberOfPages, prev, next, items) {
                 var self = this;
                 self.totalItems = ko.observable(totalItems);
                 self.numberOfPages = ko.observable(numberOfPages);
                 self.prev = ko.observable(prev);
                 self.next = ko.observable(next);
                 self.items = ko.observableArray(ko.utils.arrayMap(items, function (q) {
                     return new QItem(q.link, q.id, q.title, q.body);
                 }));
             }
     
             function QListx(totalItems, numberOfPages, prev, next, items) {
                 var self = this;
                 self.totalItems = ko.observable(totalItems);
                 self.numberOfPages = ko.observable(numberOfPages);
                 self.prev = ko.observable(prev);
                 self.next = ko.observable(next);
                 self.items = items;
     
             }*/


        let questionlist = ko.observableArray([]);
        let p = 1;
        let ps = 2;

        let loaded = ko.observable(false);

        
        bs.getBrowseItems(p, ps, function (data) {
            console.log("Data from api call search : ", data);
            if (data) {
                loaded(true);
                questionlist(data);
            }
        });



        return {
            questionlist,
            loaded

        };
    };

});


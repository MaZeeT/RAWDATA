define(["knockout", "annotationsService", "messaging"], function (ko, as, mess) {

    return function () {

        let annolist = ko.observableArray([]);
        let p = 1; //initial page
        let ps = 5; //initial pagesize
        let pshow = ko.observable();

        let nexturi = '666'; //placeholder for grabbing querystring page=
        let prevuri = '666'; //placeholder for grabbing querystring page=

        let pgsizepreset = ko.observableArray(['5', '10', '20', '30', '40', '50']) //selection of pagesizes
        let loaded = ko.observable(false); //help with hiding elements until initial data has been loaded 
        let getpgsize = ko.observable(); //for getting new pagesize

        //grab data when pagesize change
        let pgsizechanged = function setPgSize(context) {
            console.log("getpgsiz: ", context.getpgsize());
            if (context.getpgsize()) {
                ps = context.getpgsize();
                p = 1;
                pshow(p);
                as.getAllAnnos(p, ps, function (data) {
                    console.log("Data from api call search : ", data);
                    if (data) {
                        annolist(data);
                        nexturi = data.next;
                        prevuri = data.prev;
                    }
                })
            };
        };

        //thread requested
        let selectPostItem = function (item) {
            console.log("Item.threadlink is: ", item.postUrl);
            console.log("Item is: ", item);
            mess.dispatch(mess.actions.selectPost(item.postUrl));
            console.log("In between dispatches");
            mess.dispatch(mess.actions.selectMenu("postdetails"));
        };

        //grab data when page change
        function getPg(direction) {
            let npg = null;
            if (direction == 'next') {
                npg = getParameterByName('page', nexturi);
            } else if (direction == 'prev') { npg = getParameterByName('page', prevuri); }

            console.log("dat: ", direction);
            console.log("param: ", npg);
            if (npg) {
                as.getAllAnnos(npg, ps, function (data) {
                    console.log("Data from api call search : ", data);
                    if (data) {
                        p = npg;
                        pshow(p);
                        annolist(data);
                        nexturi = data.next;
                        prevuri = data.prev;
                    }
                })
            };
        };

        //return named querystring value
        function getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        }

        //grab data for initial view
        as.getAllAnnos(p, ps, function (data) {
            console.log("Data from api call search : ", data);

            if (data) {
                pshow(p);
                annolist(data);
                nexturi = data.next;
                prevuri = data.prev;
                loaded(true); 
            }
        });

        //stuff available for binding
        return {
            annolist,
            getPg,
            pgsizepreset,
            selectPostItem,
            getpgsize,
            pgsizechanged,
            pshow,
            loaded //note order matters
        };
    };

});
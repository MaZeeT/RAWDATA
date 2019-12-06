define(['knockout', 'services/BrowseService'], function (ko, bs) {

    return function () {

        let questionlist = ko.observableArray([]);
        let p = 1;
        let ps = 5;

        let nexturi = '666';
        let prevuri = '666';

        let pgsizepreset = ko.observableArray(['5', '10', '20', '30', '40', '50'])
        let loaded = ko.observable(false);
        let getpgsize = ko.observable();

        let pgsizechanged = function setPgSize(context) {
            console.log("getpgsiz: ", context.getpgsize());
            if (context.getpgsize()) {
                ps = context.getpgsize();
                p = 1;
                bs.getBrowseItems(p, ps, function (data) {
                    console.log("Data from api call search : ", data);
                    if (data) {
                        questionlist(data);
                        nexturi = data.next;
                        prevuri = data.prev;
                    }
                })
            };

        };


        function getPg(direction) {
            let npg = null;
            if (direction == 'next') {
                npg = getParameterByName('page', nexturi);
            } else if (direction == 'prev') { npg = getParameterByName('page', prevuri); }

            console.log("dat: ", direction);
            console.log("param: ", npg);
            if (npg) {
                bs.getBrowseItems(npg, ps, function (data) {
                    console.log("Data from api call search : ", data);
                    if (data) {
                        questionlist(data);
                        nexturi = data.next;
                        prevuri = data.prev;
                    }
                })
            };
        };

        function getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        }

        bs.getBrowseItems(p, ps, function (data) {
            console.log("Data from api call search : ", data);

            if (data) {
                loaded(true);
                questionlist(data);
                nexturi = data.next;
                prevuri = data.prev;

            }
        });



        return {
            questionlist,
            loaded,
            getPg,
            pgsizepreset,
            getpgsize,
            pgsizechanged

        };
    };

});


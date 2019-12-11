define(['knockout', 'browseService', 'messaging'], function (ko, bs, mess) {

    return function () {
        //detect device size
        let onSmallDevice = ko.observable(true); // not implemented yet but something
        // like window.onload that is not working should do the trick .... 

        let questionlist = ko.observableArray([]);
        let p = 1; //initial page
        let pshow = ko.observable();
        let ps = 5; //initial pagesize

        let nexturi = '666'; //placeholder for grabbing querystring page= value
        let prevuri = '666'; //placeholder for grabbing querystring page= value

        let pgsizepreset = ko.observableArray(['5', '10', '20', '30', '40', '50']); //selection of pagesizes
        let loaded = ko.observable(false); // help with hiding elements until initial data has been loaded 
        let getpgsize = ko.observable(); //for getting new pagesize


        //comp change requested
        function changeComp(component) {
            if (component === 'search') {
                mess.dispatch(mess.actions.selectMenu("Home"));
            } else if (component === 'wordcloud') {
                mess.dispatch(mess.actions.selectMenu("wordcloud"));
            } 
        };

        //thread requested
        let selectPostItem = function (item) {
            console.log("Item.threadlink is: ", item.link);
            console.log("Item is: ", item);
            mess.dispatch(mess.actions.selectPost(item.link));
            console.log("In between dispatches");
            mess.dispatch(mess.actions.selectMenu("postdetails"));
        };


        //grab data when pagesize change
        let pgsizechanged = function setPgSize(context) {
            console.log("getpgsiz: ", context.getpgsize());
            console.log("Size: ", window.innerWidth);
            if (context.getpgsize()) {
                ps = context.getpgsize();
                p = 1;
                pshow(p);
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

        //grab data when page change
        function getPg(direction) {
            let npg = null;
            if (direction === 'next') {
                npg = getParameterByName('page', nexturi);
            } else if (direction === 'prev') { npg = getParameterByName('page', prevuri); }

            console.log("dat: ", direction);
            console.log("param: ", npg);
            if (npg) {
                bs.getBrowseItems(npg, ps, function (data) {
                    console.log("Data from api call search : ", data);
                    if (data) {
                        p = npg;
                        pshow(p);
                        questionlist(data);
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
        bs.getBrowseItems(p, ps, function (data) {
            console.log("Data from api call search : ", data);

            if (data) {
                pshow(p);
                questionlist(data);
                nexturi = data.next;
                prevuri = data.prev;
                loaded(true);
            }
        });
        
        //stuff available for binding
        return {
            onSmallDevice,
            questionlist,
            getPg,
            pgsizepreset,
            getpgsize,
            pgsizechanged,
            changeComp,
            selectPostItem,
            pshow,
            loaded //note order matters
        };
    };

});


define(['knockout', 'browseService', 'messaging', 'util'], function (ko, bs, mess, util) {

    return function () {
        //detect device size
        //let onSmallDevice = ko.observable(true); // not implemented yet but something
        // like window.onload that is not working should do the trick .... 

        let questionlist = ko.observableArray([]);
        let p = 1; //initial page
        let pshow = ko.observable();

        let nexturi = '666'; //placeholder for grabbing querystring page= value
        let prevuri = '666'; //placeholder for grabbing querystring page= value

        let pgsizepreset = ko.observableArray(['5', '10', '20', '30', '40', '50']); //selection of pagesizes
        let loaded = ko.observable(false); // help with hiding elements until initial data has been loaded 
        let getpgsize = ko.observable('10'); //for getting new pagesize
        let ps = getpgsize(); //initial pagesize

        //comp change requested; switch view
        function changeComp(component) {
            if (component === 'search') {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu("Home"));
            } else if (component === 'wordcloud') {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu("wordcloud"));
            } else if (component === 'previous' && storedPreviousView) {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu(storedPreviousView));
            }
        };

        //thread requested; switch to thread view
        let selectPostItem = function (item) {
            saveStuff();
            mess.dispatch(mess.actions.selectPost(item.link));
            mess.dispatch(mess.actions.selectMenu("postdetails"));
        };

        //grab/refresh data when pagesize change
        let pgsizechanged = function () {
            if (getpgsize()) {
                ps = getpgsize();
                p = 1;
                pshow(p);
                getBrowsing(p, ps);
            };
        };

        //grab/refresh data when page change
        function getPg(direction) {
            let npg = null;
            if (direction === 'next') {
                npg = util.getParameterByName('page', nexturi);
            } else if (direction === 'prev') { npg = util.getParameterByName('page', prevuri); }

            console.log("dat: ", direction);
            console.log("param: ", npg);
            if (npg) {
                getBrowsing(npg, ps);
            };
        };

        //get all of browsepage
        function getBrowsing(npg, ps) {
            bs.getBrowseItems(npg, ps, function (data) {
                console.log("Data from api call search : ", data);
                if (data) {
                    p = npg;
                    pshow(p);
                    questionlist(data);
                    nexturi = data.next;
                    prevuri = data.prev;
                    loaded(true);
                    saveStuff();
                }
            })
        };

        //store stuff from this view
        let saveStuff = function () {
            mess.dispatch(mess.actions.selectCurrentPage(p));
            mess.dispatch(mess.actions.selectMaxPages(ps));
            mess.dispatch(mess.actions.selectPreviousView("Browse"));
        }

        //run when changeing to this view
        //get previous component/view
        let storedPreviousView = mess.getState().selectedPreviousView;

        //store current component name
        mess.dispatch(mess.actions.selectPreviousView("Browse"));

        //restore fields
        let storedMaxPages = mess.getState().selectedMaxPages;
        let storedCurrentPage = mess.getState().selectedCurrentPage;

        if (storedPreviousView == "Browse" && (storedCurrentPage)) { p=storedCurrentPage; }
        if (storedMaxPages) {
            ps = storedMaxPages;
            getpgsize(ps);
        }

        //include buttons
        mess.actions.selectMenu("prebuttcomp");
        
        //grab data for initial view
        getBrowsing(p, ps);




        //stuff available for binding
        return {
            //onSmallDevice,
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


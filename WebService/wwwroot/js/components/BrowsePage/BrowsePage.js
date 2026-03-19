define(['knockout', 'browseService', 'messaging', 'util'], function (knockout, brownService, messaging, util) {

    return function () {

        let questionlist = knockout.observableArray([]);
        let p = 1; //initial page
        let pshow = knockout.observable();

        let nexturi = '666'; //placeholder for grabbing querystring page= value
        let prevuri = '666'; //placeholder for grabbing querystring page= value

        let pgsizepreset = knockout.observableArray(['5', '10', '20', '30', '40', '50']); //selection of pagesizes
        let loaded = knockout.observable(false); // help with hiding elements until initial data has been loaded 
        let getpgsize = knockout.observable('10'); //for getting new pagesize
        let ps = getpgsize(); //initial pagesize

        //comp change requested; switch view
        function changeComp(component) {
            if (component === 'search') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Search"));
            } else if (component === 'wordcloud') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("WordCloud"));
            } else if (component === 'previous' && storedPreviousView) {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu(storedPreviousView));
            }
        }

        //thread requested; switch to thread view
        let selectPostItem = function (item) {
            saveStuff();
            messaging.dispatch(messaging.actions.selectPost(item.link));
            messaging.dispatch(messaging.actions.selectMenu("postdetails"));
        };

        //grab/refresh data when pagesize change
        let pgsizechanged = function () {
            if (getpgsize()) {
                ps = getpgsize();
                p = 1;
                pshow(p);
                getBrowsing(p, ps);
            }
        };

        //grab/refresh data when page change
        function getPg(direction) {
            let npg = null;
            if (direction === 'next') {
                npg = util.getParameterByName('page', nexturi);
            } else if (direction === 'prev') { npg = util.getParameterByName('page', prevuri); }

            if (npg) {
                getBrowsing(npg, ps);
            }
        }

        //get all of browsepage
        function getBrowsing(npg, ps) {
            brownService.getBrowseItems(npg, ps, function (data) {
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
        }

        //store stuff from this view
        let saveStuff = function () {
            messaging.dispatch(messaging.actions.selectCurrentPage(p));
            messaging.dispatch(messaging.actions.selectMaxPages(ps));
            messaging.dispatch(messaging.actions.selectPreviousView("Browse"));
        };

        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = messaging.getState().selectedPreviousView;
            //store current component name
            messaging.dispatch(messaging.actions.selectPreviousView("Browse"));
            //restore fields
            let storedMaxPages = messaging.getState().selectedMaxPages;
            let storedCurrentPage = messaging.getState().selectedCurrentPage;
            if (storedPreviousView == "Browse" && (storedCurrentPage)) { p = storedCurrentPage; }
            if (storedMaxPages) {
                ps = storedMaxPages;
                getpgsize(ps);
            }
        };

        //execute on coming to this view
        let storedPreviousView;
        restoreStuff();
        saveStuff();
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


define(["knockout", "searchHistoryService", "messaging", "util"], function (knockout, searchHistoryService, messaging, util) {

    return function () {

        let deletedAnnotStatus = knockout.observable(false);

        let annolist = knockout.observableArray([]);
        let p = 1; //initial page
        let pshow = knockout.observable();

        let nexturi = '666'; //placeholder for grabbing querystring page=
        let prevuri = '666'; //placeholder for grabbing querystring page=

        let pgsizepreset = knockout.observableArray(['5', '10', '20', '30', '40', '50']) //selection of pagesizes
        let loaded = knockout.observable(false); //help with hiding elements until initial data has been loaded 
        let getpgsize = knockout.observable(10); //for getting new pagesize
        let ps = getpgsize(); //initial pagesize

        //grab data when pagesize change
        let pgsizechanged = function setPgSize() {
            if (getpgsize()) {
                ps = getpgsize();
                p = 1;
                pshow(p);
                getSearchHistory(p, ps);
            }
        };

        //thread requested, switch to thread view
        let selectSearchHistoryItem = function (item) {
            saveStuff();

            let stype = resolveHelper(item)[1];
            let s = item.searchString;
            let gotoSearch = resolveHelper(item)[0];

            messaging.dispatch(messaging.actions.selectSearchOptions(stype));
            messaging.dispatch(messaging.actions.selectSearchTerms(s));

            if (gotoSearch) {
                messaging.dispatch(messaging.actions.selectMenu(gotoSearch));
            }
        };

        //grab data when page change
        function getPg(direction) {
            let npg = null;
            if (direction == 'next') {
                npg = util.getParameterByName('page', nexturi);
            } else if (direction == 'prev') { npg = util.getParameterByName('page', prevuri); }

            if (npg) {
                getSearchHistory(npg, ps);
            }
        }

        //delete all search history
        let deleteSearchHistory = function () {
            searchHistoryService.deleteSearchHistory(function (serverResponse) {
                let status = serverResponse.status;
                if (status === 200) {
                    p = 1;
                    pshow(p);
                    getSearchHistory(p, ps);
                    deletedAnnotStatus(true);
                } else {
                    deletedAnnotStatus(false);
                }
            });
        };

        //get all annos
        function getSearchHistory(npg, ps) {
            searchHistoryService.getSearchHist(npg, ps, function (data) {
                if (data) {

                    if (data.status == 400 || data.status == 666) {
                        //bad request / incomplete json/weird response
                        return;
                    }

                    if (data.status == 401) {
                        //unauthorized, goto login page
                        changeComp('unauth');
                        return;
                    } else {
                        //ok so far
                        p = npg;
                        pshow(p);
                        annolist(data);
                        nexturi = data.next;
                        prevuri = data.prev;
                        loaded(true);
                        saveStuff();
                    }
                }
            });
        }

        let resolveHelper = function (item) {
            let stype = item.searchMethod;

            let gotoSearch;
            let methodUsed;

            if (stype == "tfidf") {
                methodUsed = "TFIDF";
                gotoSearch = "Search";
            }
            else if (stype == "bestmatch") {
                methodUsed = "Best Match";
                gotoSearch = "Search";
            }
            else if (stype == "exactmatch") {
                methodUsed = "Exact Match";
                gotoSearch = "Search";
            }
            else if (stype == "simple") {
                methodUsed = "Simple Match";
                gotoSearch = "Search";
            }
            else if (stype == "wordsbest") {
                methodUsed = "Best Match";
                gotoSearch = "WordCloud";
            }
            else if (stype == "wordstfidf") {
                methodUsed = "TFIDF";
                gotoSearch = "WordCloud";
            } else if (!stype) {
                methodUsed = "";
                gotoSearch = "";
            }
            return [gotoSearch, methodUsed];
        };

        let resolveSearchMethod = function (item) {
            let pagename = resolveHelper(item)[0];
            let methodname = resolveHelper(item)[1];
            return (pagename.concat(' - ', methodname));
        };

        //comp change requested
        function changeComp(component) {
            if (component === 'history') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("History"));
            } else if (component === 'book') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Annotations"));
            } else if (component === 'anno') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Bookmarks"));
            } else if (component === 'unauth') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("authentication"));
            } else if (component === 'previous' && storedPreviousView) {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu(storedPreviousView));
            }
        }

        //store stuff from this view
        let saveStuff = function () {
            messaging.dispatch(messaging.actions.selectCurrentPage(p));
            messaging.dispatch(messaging.actions.selectMaxPages(ps));
            //store current component name
            messaging.dispatch(messaging.actions.selectPreviousView("Search History"));
        };

        //restore stuff to this view
        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = messaging.getState().selectedPreviousView;
            //restore fields
            let storedMaxPages = messaging.getState().selectedMaxPages;
            let storedCurrentPage = messaging.getState().selectedCurrentPage;

            if (storedPreviousView == "Search History" && (storedCurrentPage)) { p = storedCurrentPage; }
            if (storedMaxPages) {
                ps = storedMaxPages;
                getpgsize(ps);
            }
        };

        //run when changing to this view

        let storedPreviousView;
        restoreStuff();
        saveStuff();

        //grab data for initial view
        getSearchHistory(p, ps);

        //stuff available for binding
        return {
            resolveSearchMethod,
            deleteSearchHistory,
            deletedAnnotStatus,
            annolist,
            getPg,
            pgsizepreset,
            selectSearchHistoryItem,
            getpgsize,
            pgsizechanged,
            pshow,
            changeComp,
            loaded //note order matters
        };
    };

});
define(["knockout", "searchHistoryService", "messaging", "postservice", "util"], function (ko, shs, mess, postservice, util) {

    return function () {

        // let postUrl = ko.observable(mess.getState().selectedPost);
        let updateAnnotationValue = ko.observable("");
        let deletedAnnotStatus = ko.observable(false);

        let annolist = ko.observableArray([]);
        let p = 1; //initial page
        let pshow = ko.observable();

        let nexturi = '666'; //placeholder for grabbing querystring page=
        let prevuri = '666'; //placeholder for grabbing querystring page=

        let pgsizepreset = ko.observableArray(['5', '10', '20', '30', '40', '50']) //selection of pagesizes
        let loaded = ko.observable(false); //help with hiding elements until initial data has been loaded 
        let getpgsize = ko.observable(10); //for getting new pagesize
        let ps = getpgsize(); //initial pagesize

        //grab data when pagesize change
        let pgsizechanged = function setPgSize(context) {
            console.log("getpgsiz: ", context.getpgsize());
            if (context.getpgsize()) {
                ps = context.getpgsize();
                p = 1;
                pshow(p);
                getSearchHistory(p, ps);
            };
        };

        //thread requested, switch to thread view
        let selectSearchHistoryItem = function (item) {
            saveStuff();
            console.log("item searchmethod: ", item.searchMethod);
            console.log("item searchrerms: ", item.searchString);

            let stype = resolveHelper(item)[1];
            let s = item.searchString;
            let gotoSearch = resolveHelper(item)[0];

           // let pagename = resolveHelper(item)[0];
           // let methodname = resolveHelper(item)[1];

          /*  if (stype == "tfidf") {
                mess.dispatch(mess.actions.selectSearchOptions("TFIDF"));
                gotoSearch = "Search";
            }
            else if (stype == "bestmatch") {
                mess.dispatch(mess.actions.selectSearchOptions("Best Match"));
                gotoSearch = "Search";
            }
            else if (stype == "exactmatch") {
                mess.dispatch(mess.actions.selectSearchOptions("Exact Match"));
                gotoSearch = "Search";
            }
            else if (stype == "simple") {
                mess.dispatch(mess.actions.selectSearchOptions("Simple Match"));
                gotoSearch = "Search";
            }
            else if (stype == "wordsbest") {
                mess.dispatch(mess.actions.selectSearchOptions("best"));
                gotoSearch = "WordCloud";
            }
            else if (stype == "wordstfidf") {
                mess.dispatch(mess.actions.selectSearchOptions("tfidf"));
                gotoSearch = "WordCloud";
            }*/

            mess.dispatch(mess.actions.selectSearchOptions(stype));
            mess.dispatch(mess.actions.selectSearchTerms(s));

            if (gotoSearch) {
                mess.dispatch(mess.actions.selectMenu(gotoSearch));
            }
        };

        //grab data when page change
        function getPg(direction) {
            let npg = null;
            if (direction == 'next') {
                npg = util.getParameterByName('page', nexturi);
            } else if (direction == 'prev') { npg = util.getParameterByName('page', prevuri); }

            console.log("dat: ", direction);
            console.log("param: ", npg);
            if (npg) {
                getSearchHistory(npg, ps);
            };
        };


        //delete annotation
        let deleteAnnotation = function (value) {
            if (value.annotationId) {
                let annotationId = value.annotationId;
                postservice.deleteAnnotation(annotationId, function (serverResponse) {
                    let status = serverResponse.status;
                    console.log("Server response: ", serverResponse);
                    if (status === 200) {
                        getAnnos(p, ps);
                        updateAnnotationValue("");
                        deletedAnnotStatus(true);
                    } else {
                        deletedAnnotStatus(false);
                    }
                });
            } else {
                deletedAnnotStatus(false);
            }
        };

        //get all annos
        function getSearchHistory(npg, ps) {
            shs.getSearchHist(npg, ps, function (data) {
                console.log("Data from api call search : ", data);
                if (data) {
                    p = npg;
                    pshow(p);
                    annolist(data);
                    nexturi = data.next;
                    prevuri = data.prev;
                    loaded(true);
                    //resolveSearchType(data)
                    saveStuff();
                }
            });
        };

        let resolveHelper = function (item) {
            let stype = item.searchMethod;
            //let s = item.searchString;
            let gotoSearch;
            let methodUsed;

            if (stype == "tfidf") {
                methodUsed="TFIDF";
                gotoSearch = "Search";
            }
            else if (stype == "bestmatch") {
                methodUsed ="Best Match";
                gotoSearch = "Search";
            }
            else if (stype == "exactmatch") {
                methodUsed ="Exact Match";
                gotoSearch = "Search";
            }
            else if (stype == "simple") {
                methodUsed ="Simple Match";
                gotoSearch = "Search";
            }
            else if (stype == "wordsbest") {
                methodUsed ="Best Match";
                gotoSearch = "WordCloud";
            }
            else if (stype == "wordstfidf") {
                methodUsed ="TFIDF";
                gotoSearch = "WordCloud";
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
                mess.dispatch(mess.actions.selectMenu("History"));
            } else if (component === 'book') {
                saveStuff()
                mess.dispatch(mess.actions.selectMenu("Annotations"));
            } else if (component === 'anno') {
                saveStuff()
                mess.dispatch(mess.actions.selectMenu("Bookmarks"));
            }else if (component === 'previous' && storedPreviousView) {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu(storedPreviousView));
            }
        };


        //store stuff from this view
        let saveStuff = function () {
            mess.dispatch(mess.actions.selectCurrentPage(p));
            mess.dispatch(mess.actions.selectMaxPages(ps));
            //store current component name
            mess.dispatch(mess.actions.selectPreviousView("Search History"));
        };

        //restore stuff to this view
        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = mess.getState().selectedPreviousView;
            //restore fields
            let storedMaxPages = mess.getState().selectedMaxPages;
            let storedCurrentPage = mess.getState().selectedCurrentPage;

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

        //include buttons
      //  mess.actions.selectMenu("hisbuttcomp");

        //grab data for initial view
        getSearchHistory(p, ps);

        //stuff available for binding
        return {
            resolveSearchMethod,
            deleteAnnotation,
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
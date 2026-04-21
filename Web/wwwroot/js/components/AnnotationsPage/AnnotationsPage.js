define(["knockout", "annotationsService", "messaging", "postservice", "util"], function (knockout, annotationsService, messaging, postservice, util) {

    return function () {
        
        let updateAnnotationValue = knockout.observable("");
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
        let pgsizechanged = function setPgSize(context) {
            if (context.getpgsize()) {
                ps = context.getpgsize();
                p = 1;
                pshow(p);
                getAnnos(p, ps);
            }
        };

        //thread requested, switch to thread view
        let selectPostItem = function (item) {
            saveStuff();
            messaging.dispatch(messaging.actions.selectPost(item.postUrl));
            messaging.dispatch(messaging.actions.selectMenu("postdetails"));
        };

        //grab data when page change
        function getPg(direction) {
            let npg = null;
            if (direction == 'next') {
                npg = util.getParameterByName('page', nexturi);
            } else if (direction == 'prev') { npg = util.getParameterByName('page', prevuri); }

            if (npg) {
                getAnnos(npg, ps);
            }
        }

        //update anno
        let updateAnnotation = function (value) {
            if (updateAnnotationValue() && value.annotationId) {
                let annotationId = value.annotationId;
                let annotationBody = updateAnnotationValue();
                postservice.updateAnnotation(annotationId, annotationBody, function (serverResponse) {
                    let status = serverResponse.status;
                    if (status === 204) {
                        getAnnos(p, ps);
                        updateAnnotationValue("");
                    }
                });
            }
        };

        //delete annotation
        let deleteAnnotation = function (value) {
            if (value.annotationId) {
                let annotationId = value.annotationId;
                postservice.deleteAnnotation(annotationId, function (serverResponse) {
                    let status = serverResponse.status;
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
        function getAnnos(npg, ps) {
            annotationsService.getAllAnnos(npg, ps, function (data) {
                if (data) {
                    p = npg;
                    pshow(p);
                    annolist(data);
                    nexturi = data.next;
                    prevuri = data.prev;
                    loaded(true);
                    saveStuff();
                }
            });
        }

        //comp change requested
        function changeComp(component) {
            if (component === 'history') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("History"));
            } else if (component === 'book') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Bookmarks"));
            } else if (component === 'searchhistory') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Search History"));
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
            messaging.dispatch(messaging.actions.selectPreviousView("Annotations"));
        };

        //restore stuff to this view
        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = messaging.getState().selectedPreviousView;
            //restore fields
            let storedMaxPages = messaging.getState().selectedMaxPages;
            let storedCurrentPage = messaging.getState().selectedCurrentPage;

            if (storedPreviousView == "Annotations" && (storedCurrentPage)) { p = storedCurrentPage; }
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
        getAnnos(p, ps);

        //stuff available for binding
        return {
            updateAnnotation,
            updateAnnotationValue,
            deleteAnnotation,
            deletedAnnotStatus,
            annolist,
            getPg,
            pgsizepreset,
            selectPostItem,
            getpgsize,
            pgsizechanged,
            pshow,
            changeComp,
            loaded //note order matters
        };
    };

});
define(["knockout", "annotationsService", "messaging", "postservice", "util"], function (ko, as, mess, postservice, util) {

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
                getAnnos(p, ps);
            };
        };

        //thread requested, switch to thread view
        let selectPostItem = function (item) {
            saveStuff();
            mess.dispatch(mess.actions.selectPost(item.postUrl));
            mess.dispatch(mess.actions.selectMenu("postdetails"));
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
                getAnnos(npg, ps);
            };
        };

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
        function getAnnos(npg, ps) {
            as.getAllAnnos(npg, ps, function (data) {
                console.log("Data from api call search : ", data);
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
        };

        //comp change requested
        function changeComp(component) {
            if (component === 'history') {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu("History"));
            } else if (component === 'book') {
                saveStuff()
                mess.dispatch(mess.actions.selectMenu("Bookmarks"));
            } else if (component === 'searchhistory') {
                saveStuff()
                mess.dispatch(mess.actions.selectMenu("Search History"));
            } else if (component === 'previous' && storedPreviousView) {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu(storedPreviousView));
            }
        };


        //store stuff from this view
        let saveStuff = function () {
            mess.dispatch(mess.actions.selectCurrentPage(p));
            mess.dispatch(mess.actions.selectMaxPages(ps));
            //store current component name
            mess.dispatch(mess.actions.selectPreviousView("Annotations"));
        };

        //restore stuff to this view
        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = mess.getState().selectedPreviousView;
            //restore fields
            let storedMaxPages = mess.getState().selectedMaxPages;
            let storedCurrentPage = mess.getState().selectedCurrentPage;

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

        //include buttons
     //   mess.actions.selectMenu("hisbuttcomp");

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
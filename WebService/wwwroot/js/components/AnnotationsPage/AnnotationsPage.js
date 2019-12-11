define(["knockout", "annotationsService", "messaging", "postservice", "util"], function (ko, as, mess, postservice, util) {

    return function () {

       // let postUrl = ko.observable(mess.getState().selectedPost);
        let updateAnnotationValue = ko.observable("");
        let deletedAnnotStatus = ko.observable(false);

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
                getAnnos(p, ps);
            };
        };

        //thread requested, switch to thread view
        let selectPostItem = function (item) {
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
            //console.log("This is new: ", updateAnnotationValue());
            //console.log("This is val: ", value.annotationId);
            if (updateAnnotationValue() && value.annotationId) {
                //console.log("This is valsth: ", value.annotationId);
                ///console.log("Now one can update the selected annotation with data: ", value);
                let annotationId = value.annotationId;
                let annotationBody = updateAnnotationValue();
                postservice.updateAnnotation(annotationId, annotationBody, function (serverResponse) {
                    let status = serverResponse.status;
                    if (status === 204) {
                        getAnnos(p, ps);
                        //callServiceGetThread(postUrl());
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
                        //console.log("posturl: ", postUrl());
                        getAnnos(p, ps);
                      //  callServiceGetThread(postUrl());
                        updateAnnotationValue("");
                       // callServiceGetThread(postUrl());
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
                }
            })
        };

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
            loaded //note order matters
        };
    };

});
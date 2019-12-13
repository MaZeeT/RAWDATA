define(['knockout', 'postservice', 'messaging'], function (ko, postservice ,messaging) {
    return function () {
        let postUrl = ko.observable(messaging.getState().selectedPost);
        let annotationBodyText = ko.observable("");
        let annotatedPostValues = ko.observable();
        let updateAnnotationValue = ko.observable("");

        let postDetails = ko.observable([]);
        let postAnnotationsArray = ko.observable([]);
        let showspinner = ko.observable(true);
        let showAnnotTextArea = ko.observable(false);
        let responseData = ko.observable(false);
        let deletedAnnotStatus = ko.observable(false);
        let newAnnotation = ko.observable({});

        

        postservice.getAllChildDataOfPostUrl(postUrl(), function (responseFromServer) {
            if (responseFromServer) {
                console.log("What is passed to postDetails: ", responseFromServer[0].annotations);
                postDetails(responseFromServer);
                console.log("What is passed to postDetails: ", postDetails()[1].annotations);
                postAnnotationsArray(responseFromServer)
                showspinner(false);
            }
        });

        let addAnnotation = function (value, event) {
            console.log("value ", value);
            console.log("you clicked " + event.target.id);
            console.log(showAnnotTextArea());
            showAnnotTextArea(true);
        };

        
        let addBookmark = function (value) {
            console.log(value.id, value.createBookmarkLink);
            const createBookmarkUrl = value.createBookmarkLink;
            postservice.savePostAsBookmark(createBookmarkUrl, function (responseFromServer) {
                responseData(responseFromServer);
            });
        };


        let updateAnnotation = function (value) {
            console.log("This is new: ", updateAnnotationValue());
            if (updateAnnotationValue() && value.annotationId) {
                console.log("Now one can update the selected annotation with data: ", value);
                let annotationId = value.annotationId;
                let annotationBody = updateAnnotationValue();
                postservice.updateAnnotation(annotationId, annotationBody, function (serverResponse) {
                    let status = serverResponse.status;
                    if (status === 204) {
                        callServiceGetThread(postUrl());
                        updateAnnotationValue("");
                    }
                });
            }
        }

        let deleteAnnotation = function (value) {

            if (value.annotationId) {
                let annotationId = value.annotationId;
                postservice.deleteAnnotation(annotationId, function (serverResponse) {
                    let status = serverResponse.status;
                    console.log("Serv response: ", serverResponse);
                    if (status === 200) {
                        //callServiceGetThread(postUrl());
                        updateAnnotationValue("");
                        callServiceGetThread(postUrl());
                        deletedAnnotStatus(true);
                    } else {
                        deletedAnnotStatus(false);
                    }
                });

            } else {
                deletedAnnotStatus(false);
            }

        };
       


        annotationBodyText.subscribe(function (annotBody) {
            console.log("Value from test: ", annotBody);
            console.log("myStinkingValue: ", annotatedPostValues());
            
            if (annotBody.length === 0 ) {
                return;
            } 
            const createAnnotObject = {
                postid: annotatedPostValues().id,
                annotBody
            };

            postservice.saveAnnotationOnPost(createAnnotObject, function (responseFromServer) {
                if (responseFromServer) {
                    console.log("Moni, you were a smart kookie here :D ", responseFromServer);
                    annotationBodyText("");
                    newAnnotation(responseFromServer)
                    console.log("what i wanna do: ", newAnnotation());
                    callServiceGetThread(postUrl());
                }

            });
        });
 

        function callServiceGetThread(postUrl) {
            postservice.getAllChildDataOfPostUrl(postUrl, function (responseFromServer) {
                if (responseFromServer) {
                    console.log("What is passed to postDetails: ", responseFromServer[0].annotations);
                    postDetails(responseFromServer);
                    console.log("What is passed to postDetails: ", postDetails()[1].annotations);
                    postAnnotationsArray(responseFromServer)
                    showspinner(false);
                }
            });
        }

        //comp change requested
        function changeComp(component) {
            if (component === 'previous' && storedPreviousView) {
               // saveStuff();
                messaging.dispatch(messaging.actions.selectMenu(storedPreviousView));
            }
        };

        //get previous component/view
        let storedPreviousView = messaging.getState().selectedPreviousView;

        //store current component name
        //messaging.dispatch(messaging.actions.selectPreviousView("postdetails"));
        messaging.actions.selectMenu("prebuttcomp");

        return {
            changeComp,
            postUrl,
            postDetails,
            addAnnotation,
            addBookmark,
            showspinner,
            showAnnotTextArea,
            responseData,
            annotationBodyText,
            annotatedPostValues,
            newAnnotation,
            updateAnnotationValue,
            updateAnnotation,
            deleteAnnotation,
            deletedAnnotStatus
        };
    };
});
define(['knockout', 'postservice', 'messaging'], function (ko, postservice, mess) {
    return function () {

      /*  let searchpart = ko.observable(false); // help with hiding elements until initial data has been loaded 
        let historypart = ko.observable(false); // help with hiding elements until initial data has been loaded 



        //get previous component/view
        let storedPreviousView = mess.getState().selectedPreviousView;
        console.log("included button ", storedPreviousView);
        //store current component name
       // mess.dispatch(mess.actions.selectPreviousView("postdetails"));

        if (storedPreviousView == "Browse" || storedPreviousView == "wordcloud" || storedPreviousView == "Home") {
            searchpart(true);
            historypart(false);
        } else if (storedPreviousView == "History" || storedPreviousView == "Annotations" || storedPreviousView == "Bookmarks") {
            searchpart(false);
            historypart(true);
        }
        else {
            searchpart(false);
            historypart(false);
        }*/


        return {
         //   searchpart,
          //  historypart
        };
    };
});
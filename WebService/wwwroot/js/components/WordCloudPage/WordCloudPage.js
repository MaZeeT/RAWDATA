define(['knockout', 'wordCloudService', 'messaging', "jqcloud"], function (ko, wc, mess) {

    return function () {

        let noofresults = ko.observableArray(['5', '10', '20', '30', '40', '50', '100', '200']) //selection of results todo: make into slider or sth
        let loaded = ko.observable(false); // help with hiding elements until initial data has been loaded 
        let getnoofresults = ko.observable(); //for getting new number of results to grab

        let max = 100;
        let stype = 4;

        let width = 200;
        let height = 200;

        const placeholderStr = "Search with terms here..."
        let searchTerms = ko.observable(placeholderStr);

        let searchResult = ko.observableArray([]);
     //   let showTable = ko.observable(false);
     //   let totalResults = ko.observable("0");


        //comp change requested
        function changeComp(component) {
            if (component === 'search') {
                mess.dispatch(mess.actions.selectMenu("Home"));
            } else if (component === 'browse') {
                mess.dispatch(mess.actions.selectMenu("Browse"));
            }
        };


        //max results
        let noofresultschanged = function setPgSize(context) {
            console.log("getmax: ", context.getnoofresults());
            if (context.getnoofresults()) {
                max = context.getnoofresults();
            };
        };


        searchTerms.subscribe(function (searchStr) {
            if (searchStr.length === 0) {
                searchResult([]);
                return;
            }

            wc.getWCItems(searchStr, stype, max, function (data) {
                console.log("Data from api call search : ", data);

                if (data) {
                    searchResult(data);

                    data1 = data.map(function (a) {
                        return { text: a.term, weight: a.rank };
                    });
                    console.log("datamap: ", data1);

                    $('#cloud').jQCloud(data1,
                        {
                            autoResize: true
                        });

                    loaded(true);
                }
            });
        });

     /*   let selectSearchResultItem = function (item) {
            console.log("Item.threadlink is: ", item.threadLink);
            mess.dispatch(mess.actions.selectPost(item.threadLink));
            console.log("In between dispatches");
            mess.dispatch(messaging.actions.selectMenu("postdetails"));
        };*/


                     


        return {
            searchTerms,
            searchResult,
            getnoofresults,
            noofresults,
            changeComp,
            noofresultschanged,
            loaded

        }
    }

});
define(['knockout', 'wordCloudService', 'messaging', "jqcloud"], function (ko, wc, mess) {

    return function () {

       // let noofresults = ko.observableArray(['5', '10', '20', '30', '40', '50', '100', '200']) //selection of results todo: make into slider or sth
        let loaded = ko.observable(false); // help with hiding elements until initial data has been loaded 
     //   let getnoofresults = ko.observable(); //for getting new number of results to grab

        let max = 15;
        let stype = 4;

        let selectedValue = ko.observable(15);

     //   let width = 200;
     //   let height = 200;

        const placeholderStr = "Search with terms here..."
        let searchTerms = ko.observable(placeholderStr);

        let searchResult = ko.observableArray([]);
     //   let showTable = ko.observable(false);
     //   let totalResults = ko.observable("0");

      /*  function enter(data, event) {
            if (event.which == 13) {
                //call method here
                console.log('Enter Key Pressed!');
            }
        }*/



        //comp change requested
        function changeComp(component) {
            if (component === 'search') {
                mess.dispatch(mess.actions.selectMenu("Home"));
            } else if (component === 'browse') {
                mess.dispatch(mess.actions.selectMenu("Browse"));
            }
        };


        //max results
     /*   let noofresultschanged = function setPgSize(context) {
            console.log("getmax: ", context.getnoofresults());
            if (context.getnoofresults()) {
                max = context.getnoofresults();
            };
        };*/

        let clrsearchfield = function upd() {
            console.log("searchreerm : ", searchTerms());
            if (searchTerms() === placeholderStr) {
                searchTerms('');
            }
            console.log("searchreerm : ", searchTerms());
        }

        let cloudupdate = function upd() {

            console.log("searchreerm : ", searchTerms());
            console.log("stypw : ", stype);
            max = selectedValue();
            console.log("maxres : ", max);
            console.log("slider value : ", selectedValue());

            wc.getWCItems(searchTerms(), stype, max, function (data) {
                console.log("Data from api call search : ", data);

                if (data) {

                    console.log("data status : ", data.status);

                    if (data.status == 400) {
                        //bad request
                        searchResult([]);
                        searchTerms('Try searching for something!');
                        return;
                    } else if (data.status == 401 || data.status == 666) {
                        //unauthorized or incomplete, goto login page
                        mess.dispatch(mess.actions.selectMenu("authentication"));
                        return;
                    } else {
                        //ok so far
                        loaded(true);
                        searchResult(data);

                        data1 = data.map(function (a) {
                            return { text: a.term, weight: a.rank };
                        });
                        console.log("datamap: ", data1);

                        $('#cloud').jQCloud('destroy');
                        $('#cloud').jQCloud(data1,
                            {
                                autoResize: true
                            });
                    }
                }
            });

        };

        searchTerms.subscribe(function (searchStr) {
            if (searchStr.length === 0) {
                searchResult([]);
                return;
            };
            max = selectedValue();
            console.log("maxres : ", max);
            console.log("slider value : ", selectedValue());

            wc.getWCItems(searchStr, stype, max, function (data) {
                console.log("Data from api call search : ", data);
                searchTerms(searchStr);
                if (data) {
                    loaded(true);
                    searchResult(data);

                    data1 = data.map(function (a) {
                        return { text: a.term, weight: a.rank };
                    });
                    console.log("datamap: ", data1);

                    $('#cloud').jQCloud('destroy'); /// cant figure out how to update lol! so am destroying it..
                    $('#cloud').jQCloud(data1,
                        {
                            autoResize: true
                        });

                   
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
         //   getnoofresults,
         //   noofresults,
            changeComp,
           // enter,
           // noofresultschanged,
            cloudupdate,
            clrsearchfield,
            selectedValue,
            loaded

        }
    }

});
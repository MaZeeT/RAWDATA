define(['knockout', 'wordCloudService', 'messaging', "jqcloud"], function (ko, wc, mess) {

    return function () {

        let loaded = ko.observable(false); // help with hiding elements until initial data has been loaded 

        let max = 15;
        let stype = 4;

        let stypebtn = ko.observable("tfidf");

        let selectedValue = ko.observable(15);

        const placeholderStr = "Input search terms here..."
        let searchTerms = ko.observable(placeholderStr);

        let searchResult = ko.observableArray([]);




        //clearing searchfield when clicked
        let clrsearchfield = function upd() {
            if (searchTerms() === placeholderStr) {
                searchTerms('');
            }
        };




        //for geting new data and updating wordcloud
        let cloudupdate = function upd() {

            saveStuff();

            max = selectedValue();
            if (stypebtn() == 'tfidf') { stype = 4; } else stype = 5;

            wc.getWCItems(searchTerms(), stype, max, function (data) {
                //console.log("Data from api call search : ", data);

                if (data) {

                    //console.log("data status : ", data.status);

                    if (data.status == 400) {
                        //bad request
                        searchResult([]);
                        searchTerms('Try searching for something!');
                        return;
                    } else if (data.status == 666) {
                        //incomplete json/weird response
                        searchResult([]);
                        searchTerms('Try again!');
                        return;
                    } else if (data.status == 401) {
                        //unauthorized, goto login page
                        changeComp('unauth');
                        //mess.dispatch(mess.actions.selectMenu("authentication"));
                        return;
                    } else {
                        //ok so far
                        loaded(true);
                        searchResult(data);

                        data1 = data.map(function (a) {
                            return { text: a.term, weight: a.rank };
                        });

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
            if (stypebtn() == 'tfidf') { stype = 4; } else stype = 5;

            wc.getWCItems(searchStr, stype, max, function (data) {
                searchTerms(searchStr);
                if (data) {
                    loaded(true);
                    searchResult(data);

                    data1 = data.map(function (a) { //map data to what jqcloud wants
                        return { text: a.term, weight: a.rank };
                    });
                    $('#cloud').jQCloud('destroy'); /// cant figure out how to update lol! so am destroying it..
                    $('#cloud').jQCloud(data1,
                        {
                            autoResize: true
                        });
                }
            });
        });

        //comp change requested
        function changeComp(component) {
            if (component === 'search') {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu("Search"));
            } else if (component === 'browse') {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu("Browse"));
            } else if (component === 'unauth') {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu("authentication"));
            } else if (component === 'previous' && storedPreviousView) {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu(storedPreviousView));
            }
        };

        //store stuff from this view
        let saveStuff = function () {
            mess.dispatch(mess.actions.selectSearchTerms(searchTerms()));
            if (storedSearchOptions) {
                if (stypebtn() == "tfidf" && storedSearchOptions != "tfidf" || stypebtn() == "tfidf" && storedSearchOptions != "TFIDF") {
                    mess.dispatch(mess.actions.selectSearchOptions(stypebtn()));
                }
            }
            mess.dispatch(mess.actions.selectMaxWords(selectedValue()));
            //store current component name
            mess.dispatch(mess.actions.selectPreviousView("WordCloud"));
        };

        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = mess.getState().selectedPreviousView;
            //restore fields
            let storedSearchTerms = mess.getState().selectedSearchTerms;
            storedSearchOptions = mess.getState().selectedSearchOptions;
            let storedMaxWords = mess.getState().selectedMaxWords;

            if (storedMaxWords) { selectedValue(storedMaxWords) }
            if (storedSearchTerms) { searchTerms(storedSearchTerms) }
            if (storedSearchOptions == "tfidf" || storedSearchOptions == "TFIDF") {
                stypebtn("tfidf")
            } else {
                stypebtn("best")
            }
        };

        //execute on coming to this view
        let storedPreviousView;
        let storedSearchOptions;
        restoreStuff();
        saveStuff();
       // mess.actions.selectMenu("searchbuttcomp");


        return {
            searchTerms,
            searchResult,
            changeComp,
            cloudupdate,
            stypebtn,
            clrsearchfield,
            selectedValue,
            loaded
        }
    }

});
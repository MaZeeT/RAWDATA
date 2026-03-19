define(['knockout', 'wordCloudService', 'messaging', "jqcloud"], function (knockout, wordCloudService, messaging) {

    return function () {

        let loaded = knockout.observable(false); // help with hiding elements until initial data has been loaded 

        let max = 15;
        let stype = 4;

        let stypebtn = knockout.observable("tfidf");
        let selectedValue = knockout.observable(15);

        const placeholderStr = "Input search terms here...";
        let searchTerms = knockout.observable(placeholderStr);

        let searchResult = knockout.observableArray([]);

        //clearing searchfield when clicked
        let clrsearchfield = function () {
            if (searchTerms() === placeholderStr) {
                searchTerms('');
            }
        };

        //for geting new data and updating wordcloud
        let cloudupdate = function () {
            saveStuff();
            max = selectedValue();
            if (stypebtn() == 'tfidf') {
                stype = 4;
            } else {
                stype = 5;
            }
            doWordRankSearch(searchTerms(), stype, max);
        };

        let doWordRankSearch = function (terms, stype, max) {
            wordCloudService.getWCItems(terms, stype, max, function (data) {
                if (data) {
                    if (data.status == 400) {
                        //bad request
                        searchResult([]);
                        searchTerms('Try searching for something!');
                        return;
                    }

                    if (data.status == 666) {
                        //incomplete json/weird response
                        searchResult([]);
                        searchTerms('Try again!');
                        return;
                    }

                    if (data.status == 401) {
                        changeComp('unauth');
                        return;
                    } else {
                        //ok so far
                        loaded(true);
                        searchResult(data);
                        doCloudUpdate(searchResult());
                    }
                }
            });
        };

        let doCloudUpdate = function (wordList) {
            data1 = wordList.map(function (a) { //map data to what jqcloud wants
                return {text: a.term, weight: a.rank};
            });
            $('#cloud').jQCloud('destroy'); /// cant figure out how to update lol! so am destroying it..
            $('#cloud').jQCloud(data1,
                {
                    autoResize: true
                });
        };

        searchTerms.subscribe(function (searchStr) {
            if (searchStr.length === 0) {
                searchResult([]);
                return;
            }
            max = selectedValue();
            if (stypebtn() == 'tfidf') {
                stype = 4;
            } else {
                stype = 5;
            }

            doWordRankSearch(searchTerms(), stype, max);
        });

        //comp change requested
        function changeComp(component) {
            if (component === 'search') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Search"));
            } else if (component === 'browse') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Browse"));
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
            messaging.dispatch(messaging.actions.selectSearchTerms(searchTerms()));
            if (storedSearchOptions) {
                if (stypebtn() == "tfidf" && storedSearchOptions != "tfidf" || stypebtn() == "tfidf" && storedSearchOptions != "TFIDF") {
                    messaging.dispatch(messaging.actions.selectSearchOptions(stypebtn()));
                }
            }
            messaging.dispatch(messaging.actions.selectMaxWords(selectedValue()));
            //store current component name
            messaging.dispatch(messaging.actions.selectPreviousView("WordCloud"));
        };

        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = messaging.getState().selectedPreviousView;
            //restore fields
            let storedSearchTerms = messaging.getState().selectedSearchTerms;
            storedSearchOptions = messaging.getState().selectedSearchOptions;
            let storedMaxWords = messaging.getState().selectedMaxWords;

            if (storedMaxWords) {
                selectedValue(storedMaxWords)
            }
            if (storedSearchTerms) {
                searchTerms(storedSearchTerms)
            }
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
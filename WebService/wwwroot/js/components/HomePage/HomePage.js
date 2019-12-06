define(["knockout", "homeService"], function (ko, ds) {

    return function () {
        const placeholderStr = "Search with terms here..."
        let searchTerms = ko.observable(placeholderStr);
        console.log(searchTerms());
        return {
            searchTerms
        }
    }

});
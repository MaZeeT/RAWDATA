define(["knockout"], function (ko) {
    var firstName = ko.observable("Peter");
    var lastName = ko.observable("Smith");

    var fullName = ko.computed(function () {
        return firstName() + " " + lastName();
    });

    var names = ko.observableArray(["Peter", "John"]);

    var addName = function (data) {
        names.push(fullName());
    };

    var delName = function (name) {
        names.remove(name);
    };

    return {
        firstName,
        lastName,
        fullName,
        names,
        addName,
        delName
    };
});
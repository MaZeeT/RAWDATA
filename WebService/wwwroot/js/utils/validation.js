define([""], function () {

    const regExUsernameValidValue = /[^a-zA-Z\d]/gmi;
    const regExPasswordInvalidValue = /[^a-zA-Z\d]/gmi;

    function isValidUsername(usernameValue) {
        const username = usernameValue;
        const result = username.match(regExUsernameValidValue);
        if (!username) {
            return false;
        }
        if (result) {
            return false;
        }
        return true;

    }

    function isValidPassword(passValue) {
        const password = passValue;
        const result = password.match(regExPasswordInvalidValue);
        if (!password || password.length < 6) {
            return false;
        }
        if (result) {
            return false;
        }
        return true;

    }

    return {
        isValidUsername,
        isValidPassword
    }

});
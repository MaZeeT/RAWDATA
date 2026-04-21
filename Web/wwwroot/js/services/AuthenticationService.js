define([""], function () {

    let getLoginUser = async function (incomingUserCredentials, callback) {
        try {
            const response = await fetch("api/auth/tokens", {
                method: 'POST', // or 'PUT'
                body: JSON.stringify(incomingUserCredentials), // data can be `string` or {object}!
                headers: {
                    'Content-Type': 'application/json'
                }
            }).then(function (response) {
                return response.json();
            }).then(function (responseBody) {
                return responseBody;
            });
            callback(response);

        } catch (error) {
            console.error('Error:', error);
        }
    };
    let signUpUser = async function (incomingUserCredentials, callback) {
        try {
            const response = await fetch("api/auth/users", {
                method: 'POST',
                body: JSON.stringify(incomingUserCredentials),
                headers: {
                    'Content-Type': 'application/json'
                }
            }).then(function (response) {
                return response;
            });
            callback(response);

        } catch (error) {
            console.error('Error:', error);
        }
    };

    return {
        getLoginUser,
        signUpUser
    }
});
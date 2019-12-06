define(["jquery"], function () {



    var getLoginUser = async function (incomingUserCredentials, callback) {
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
    var signUpUser = async function (incomingUserCredentials, callback) {
        try {
            const response = await fetch("api/auth/users", {
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

    return {
        getLoginUser
    }
});
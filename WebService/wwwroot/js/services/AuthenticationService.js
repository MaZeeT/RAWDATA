define(["jquery"], function ($) {
    //var getLoginUser = function (callback) {
    //    $.getJSON("api/auth/tokens", callback);
    //};

    const data = { Username: 'Monica', Password:'Test'};
    var getLoginUser = async function (callback) {
        //var response = await fetch("api/auth/tokens");
        //var data = await response.json();
        //callback(data);

        try {
            const response = await fetch("api/auth/tokens", {
                method: 'POST', // or 'PUT'
                body: JSON.stringify(data), // data can be `string` or {object}!
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            const response = await response.json();
            //console.log('Success:', JSON.stringify(response));
            callback(response)
        } catch (error) {
            console.error('Error:', error);
        }
    };

    return {
        getLoginUser
    }
});
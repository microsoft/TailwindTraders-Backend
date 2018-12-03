'use strict';

module.exports = (req, context) => {
    if (!req.headers.authorization) {
        context.log("No Authorization Header Provided");
        context.res = {
            status: 403,
            body: "No Authorization Header Provided"
        };

        return false;
    }

    var splitToken = req.headers.authorization.split(" ");
    var headerEmail = splitToken[0];
    var token = splitToken[1];

    if (headerEmail !== "Email" || token === undefined || token === null | token.length === 0) {
        context.log("Invalid Authorization Header");
        context.res = {
            status: 401,
            body: "Invalid Authorization Header"
        };

        return false;
    }

    return true;
};
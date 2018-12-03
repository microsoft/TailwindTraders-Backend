'use strict';

module.exports = (req, res, next) => {
    if (!req.headers.authorization) {
        return res.status(403)
            .send({ message: "No Authorization Header Provided" });
    }

    var splitToken = req.headers.authorization.split(" ");
    var headerEmail = splitToken[0];
    var token = splitToken[1];

    if (headerEmail === "Email" && token !== undefined && token !== null && token.length > 0) {
        next();
    } else {
        return res.status(401)
            .send({ message: "Invalid Authorization Header" });
    }
};
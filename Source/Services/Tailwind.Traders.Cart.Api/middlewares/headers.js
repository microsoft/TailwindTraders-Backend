const authConfig = require("../config/authConfig");

module.exports = (req, res, next) => {
    if (JSON.parse(authConfig.UseB2C)) {
        req.headers["x-tt-name"] = `${req.authInfo.name}@${req.authInfo.name}.com`;
    } else {
        req.headers["x-tt-name"] = req.decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    }
    next();
};
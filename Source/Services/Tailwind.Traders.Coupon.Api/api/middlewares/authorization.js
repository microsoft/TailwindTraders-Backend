let jwt = require('jsonwebtoken');
const config = require('../config/authConfig');

module.exports = (req, res, next) => {
    const BEARER = 'Bearer ';
    let token = req.headers['x-access-token'] || req.headers['authorization'];


    if (token && token.startsWith(BEARER)) {
        // Remove Bearer from string
        token = token.slice(7, token.length);
    }

    if (!token) {
        return res
            .status(401)
            .json({
                success: false,
                message: 'Auth token is not supplied'
            });
    }

    if (config.UseB2C) {
        var decoded = jwt.decode(token, {complete: true});
        req.decoded = decoded;
        next();
    }
    else {
        jwt.verify(token, config.SecurityKey, (err, decoded) => {
            if (err || decoded.iss != config.Issuer) {
                return res
                    .status(401)
                    .json({
                        success: false,
                        message: 'Token is not valid'
                    });
            }
            req.decoded = decoded;
            next();
        });
    }
};
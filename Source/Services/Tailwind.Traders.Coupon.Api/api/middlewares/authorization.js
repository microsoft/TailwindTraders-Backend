let jwt = require('jsonwebtoken');
const config = require('../config/authConfig');

let checkToken = (req, res, next) => {

    // Express headers are auto converted to lowercase
    let token = req.headers['x-access-token'] || req.headers['authorization'];

    if (token && token.startsWith('Bearer ')) {
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
};

module.exports = {
    checkToken: checkToken
}
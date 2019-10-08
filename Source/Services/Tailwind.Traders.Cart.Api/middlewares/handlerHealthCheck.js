'use strict';

module.exports = (req, res, next) => {
    if(req.url == '/liveness') {
        res.status(200);
        res.send('Healthy');
        return;
    }    
    next();    
};
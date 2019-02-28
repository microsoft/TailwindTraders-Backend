'use strict';

const Coupon = require('../models/coupon');

exports.allCoupons = (req, res) => {

    // Retrieve user from token
    const user = req.decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    
    Coupon.findOne({ 'id': user })
        .then(coupons => {
            if (!coupons) {
                return res.sendStatus(204);
            }
            res.json(coupons);
        })
        .catch(err => {
            console.log(`caught the error: ${err}`);
            res.status(503).json(err);
        });
};
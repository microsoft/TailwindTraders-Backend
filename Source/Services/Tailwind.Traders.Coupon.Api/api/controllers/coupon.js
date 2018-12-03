'use strict';

const Coupon = require('../models/coupon');

exports.allCoupons = (req, res) => {
    let user = req.headers.authorization.split(" ")[1];
    
    Coupon.findOne({ 'id': user })
        .then(coupons => {
            if (!coupons) {
                res.status(404).send({ message: "Not found" });
            }
            res.json(coupons);
        })
        .catch(err => {
            console.log(`caught the error: ${err}`);
            res.status(503).json(err);
        });
};
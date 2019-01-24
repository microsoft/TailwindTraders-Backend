'use strict';

const Coupon = require('../models/coupon');

exports.allCoupons = (req, res) => {
    let user = req.headers.authorization.split(" ")[1];
    
    Coupon.findOne({ 'id': user })
        .then(coupons => {
            if (!coupons) {
                res.status(204).send({ message: "Coupons not found" });
                return;
            }
            res.json(coupons);
        })
        .catch(err => {
            console.log(`caught the error: ${err}`);
            res.status(503).json(err);
        });
};
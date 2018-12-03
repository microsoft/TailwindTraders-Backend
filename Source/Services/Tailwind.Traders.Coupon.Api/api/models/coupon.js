const mongoose = require('mongoose');

let CouponSchema = new mongoose.Schema({
    id: String,
    smallCoupons: [{
        id: Number,
        image: String,
        discount: String,
        title: String,
        until: String,
        description: String
    }],
    bigCoupon: {
        id: Number,
        image: String,
        discount: String,
        title: String
    }},
    { collection: process.env.COUPON_COLLECTION });

module.exports = mongoose.model('Coupon', CouponSchema);
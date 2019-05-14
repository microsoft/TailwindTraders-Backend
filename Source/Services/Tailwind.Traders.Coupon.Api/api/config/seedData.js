'use strict';

const csv = require('csvtojson');
const path = require('path');
const mongoose = require('mongoose');
const Coupon = require('../models/coupon');

const csvUser = "../../setup/CouponItems.csv";
const csvSmall = "../../setup/Small.csv"; 
const csvBig = "../../setup/Big.csv"; 

function buildCoupons(users, smalls, bigs){
    return users.map(item => {
        return {
            id: item.user,
            smallCoupons: smalls
                .map(item => {
                    let xitem = Object.assign({}, item);
                    xitem.image = [process.env.URL_BASE, item.image].join('/');
                    return xitem;
                })
                .filter(small => item.smallCoupons.includes(small.id))
                ,
            bigCoupon: bigs
                .map(item => {
                    let xitem = Object.assign({}, item);
                    xitem.image = [process.env.URL_BASE, item.image].join('/');
                    return xitem;
                })
                .find(big => big.id === item.bigCoupon)
        };
    });
};

async function populateDb(){
    mongoose.connection.collections[process.env.COUPON_COLLECTION].estimatedDocumentCount(async function (err, count) {
        if(count === 0){
            console.log("Starting seed data...");

            var users = await csv().fromFile(path.join(__dirname, csvUser));
            var smalls = await csv().fromFile(path.join(__dirname, csvSmall));
            var bigs = await csv().fromFile(path.join(__dirname, csvBig));

            let data = buildCoupons(users, smalls, bigs);

            await Coupon.insertMany(data)
                .then(() => console.log("End seed success"))
                .catch(err => {
                    console.error(err);
                    process.exit(1);
                } );
        }
    });
}

module.exports = populateDb;
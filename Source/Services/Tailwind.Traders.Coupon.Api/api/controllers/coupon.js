"use strict";
const Coupon = require("../models/coupon");

exports.allCoupons = (req, res) => {
  const user = req.headers["x-tt-name"];

  Coupon.findOne({ id: user })
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

exports.liveness = (req, res) => {
  res.status(200);
  res.send("Healthy");
  return;
};

"use strict";

//const swaggerUi = require('swagger-ui-express');
const couponController = require("../controllers/coupon");
//const swaggerDocument = require('../../swagger.json');

exports.add = app => {
  //app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerDocument));
  //app.use('/', app.route);

  app.route("/v1/coupon").get(couponController.allCoupons);

  app.route("/liveness").get(couponController.liveness);

  app.use((req, res, next) => {
    const err = new Error("Not found");
    err.status = 404;
    next(err);
  });
};

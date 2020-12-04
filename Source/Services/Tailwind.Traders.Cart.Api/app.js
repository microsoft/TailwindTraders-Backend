const CosmosClient = require("@azure/cosmos").CosmosClient;
const ConnectionPolicy = require("@azure/cosmos").ConnectionPolicy;
const config = require("./config/config");
const authConfig = require("./config/authConfig");
const CartController = require("./routes/cartController");
const ShoppingCartDao = require("./models/shoppingCartDao");
const RecommededDao = require("./models/recommendedDao");
const ensureAuthenticated = require("./middlewares/authorization");
const ensureB2cAuthenticated = require("./middlewares/authorizationB2c");
const setHeaders = require("./middlewares/headers");
const handlerHealthCheck = require("./middlewares/handlerHealthCheck");
const cors = require("cors");
const express = require("express");
const passport = require("passport");
const BearerStrategy = require("passport-azure-ad").BearerStrategy;
const cookieParser = require("cookie-parser");
const logger = require("morgan");
const bodyParser = require("body-parser");
const indexRouter = require("./routes/index");
const appInsights = require("applicationinsights");

if (config.applicationInsightsIK) {
  appInsights.setup(config.applicationInsightsIK);
  appInsights.start();
}

const app = express();
app.use(logger("dev"));
app.use(bodyParser.json());
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cors());
app.use(cookieParser());
app.use(handlerHealthCheck);

if (JSON.parse(authConfig.UseB2C)) {
  const options = {
    identityMetadata: authConfig.identityMetadata,
    issuer: authConfig.issuer,
    clientID: authConfig.clientID,
    policyName: authConfig.policyName,
    isB2C: true,
    validateIssuer: true,
    loggingLevel: "info",
    passReqToCallback: false
  };

  const bearerStrategy = new BearerStrategy(options, function(token, done) {
    done(null, {}, token);
  });

  app.use("/", indexRouter);

  app.use(passport.initialize());
  passport.use(bearerStrategy);

  app.use(ensureB2cAuthenticated());
} else {
  app.use(ensureAuthenticated);
}

app.use(setHeaders);

console.log(`Cosmos to use is ${config.host}`);
const cosmosClientOptions = {
  endpoint: config.host,
  key: config.authKey
};

const locations = process.env.LOCATIONS;
if (locations) {
  console.log(`Preferred locations are set to: '${locations}'`);
  const connectionPolicy = new ConnectionPolicy();
  connectionPolicy.preferredLocations = locations.split(",");
  cosmosClientOptions.connectionPolicy = connectionPolicy;
}

const disableSSL =
  (process.env.DISABLE_SSL || "").toString().toLowerCase() === "true";
if (disableSSL) {
  console.log(
    "Disabling SSL verification! Caution *NEVER* use this in production!"
  );
  if (cosmosClientOptions.connectionPolicy == undefined) {
    cosmosClientOptions.connectionPolicy = new ConnectionPolicy();
  }
}

const cosmosClient = new CosmosClient(cosmosClientOptions);

const shoppingCartDao = new ShoppingCartDao(
  cosmosClient,
  config.databaseId,
  config.containerId
);

const recommendedDao = new RecommededDao(cosmosClient, config.databaseId);

const cartController = new CartController(shoppingCartDao, recommendedDao);

console.log("Begin initialization of cosmosdb " + config.host);

shoppingCartDao
  .init(err => {
    console.error(err);
  })
  .then(() => {
    console.log(`cosmosdb ${config.host} initializated`);
  })
  .catch(err => {
    console.error(err);
    console.error(
      "Shutting down because there was an error setting up the database."
    );
    process.exit(1);
  });

recommendedDao
  .init(err => {
    console.error(err);
  })
  .then(() => {
    console.log(`cosmosdb ${config.host} recommendations initializated`);
  })
  .catch(err => {
    console.error(err);
    console.error(
      "Shutting down because there was an error setting up the database."
    );
    process.exit(1);
  });

app.get("/liveness", (req, res, next) => {
  res.status(200);
  res.send("Healthy");
  return;
});

app.get("/shoppingcart", (req, res, next) =>
  cartController.getProductsByUser(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);
app.post("/shoppingcart", (req, res, next) =>
  cartController.addProduct(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);
app.post("/shoppingcart/product", (req, res, next) =>
  cartController.updateProductQuantity(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);
app.delete("/shoppingcart/product", (req, res, next) =>
  cartController.deleteItem(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);
app.get("/shoppingcart/relatedproducts", (req, res, next) =>
  cartController.getRelatedProducts(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  const err = new Error("Not Found");
  err.status = 404;
  next(err);
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get("env") === "development" ? err : {};

  // render the error page
  res.status(err.status || 500);
  return res.send({
    error: "Error",
    details: err
  });
});

module.exports = app;

const CosmosClient = require("@azure/cosmos").CosmosClient;
const ConnectionPolicy = require("@azure/cosmos").ConnectionPolicy;
const config = require("./config/config");
const CartController = require("./routes/cartController");
const ShoppingCartDao = require("./models/shoppingCartDao");
const RecommededDao = require("./models/recommendedDao");

const ensureAuthenticated = require('./middlewares/authorization');
const cors = require('cors');
const express = require('express');
const cookieParser = require('cookie-parser');
const logger = require('morgan');
const bodyParser = require("body-parser");

const indexRouter = require('./routes/index');

const app = express();

app.use(logger('dev'));
app.use(bodyParser.json());
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cors());
app.use(cookieParser());
app.use(ensureAuthenticated);
app.use('/', indexRouter);

console.log(`Cosmos to use is ${config.host}`)

const cosmosClientOptions = {
  endpoint: config.host,
  auth: {
    masterKey: config.authKey
  }
};

const locations = process.env.LOCATIONS;
if (locations) {
  console.log(`Preferred locations are set to: '${locations}'`)
  const connectionPolicy = new ConnectionPolicy();
  connectionPolicy.PreferredLocations = locations.split(',');
  cosmosClientOptions.connectionPolicy = connectionPolicy;
}

const cosmosClient = new CosmosClient(cosmosClientOptions);
const shoppingCartDao = new ShoppingCartDao(cosmosClient, config.databaseId, config.containerId);
const recommendedDao = new RecommededDao(cosmosClient, config.databaseId);
const cartController = new CartController(shoppingCartDao, recommendedDao);

console.log('Begin initialization of cosmosdb ' + config.host);
shoppingCartDao
  .init(err => {
    console.error(err);
  })
  .then(() => {
    console.log(`cosmosdb ${config.host} initializated`);
  })
  .catch(err => {
    console.error(err);
    console.error("Shutting down because there was an error settinig up the database.");
    process.exit(1);
  });

  recommendedDao.init(err => {
    console.error(err);
  })
  .then(() => {
    console.log(`cosmosdb ${config.host} recommendations initializated`);
  })
  .catch(err => {
    console.error(err);
    console.error("Shutting down because there was an error settinig up the database.");
    process.exit(1);
  });

app.get("/shoppingcart", (req, res, next) => cartController.getProductsByEmail(req, res)
  .catch(e => { console.log(e); next(e) }));
app.post("/shoppingcart", (req, res, next) => cartController.addProduct(req, res)
  .catch(e => { console.log(e); next(e) }));
app.post("/shoppingcart/product", (req, res, next) => cartController.updateProductQuantity(req, res)
  .catch(e => { console.log(e); next(e) }));
app.delete("/shoppingcart/product", (req, res, next) => cartController.deleteItem(req, res)
  .catch(e => { console.log(e); next(e) }));
app.get("/shoppingcart/relatedproducts", (req, res, next) => cartController.getRelatedProducts(req, res)
  .catch(e => { console.log(e); next(e) }));

// catch 404 and forward to error handler
app.use(function (req, res, next) {
  const err = new Error("Not Found");
  err.status = 404;
  next(err);
});

// error handler
app.use(function (err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get("env") === "development" ? err : {};

  // render the error page
  res.status(err.status || 500);
  return res.send({
    error: 'Error',
    details: err
  })
});

module.exports = app;
'use strict';

const routes = require('./config/route');
const handlerError = require('./middlewares/handlerError');
const ensureAuthenticated = require('./middlewares/authorization');

const logger = require('morgan');
const bodyParser = require('body-parser');
const cors = require('cors');
const express = require('express');
const app = express();

app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cors());
app.use(ensureAuthenticated.checkToken);

routes.add(app);

app.use(handlerError);

module.exports = app;
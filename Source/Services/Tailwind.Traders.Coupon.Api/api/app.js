'use strict';

const routes = require('./config/route');
const handlerError = require('./middlewares/handlerError');

const logger = require('morgan');
const bodyParser = require('body-parser');
const cors = require('cors');
const express = require('express');
const app = express();

app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cors());

routes.add(app);

app.use(handlerError);

module.exports = app;
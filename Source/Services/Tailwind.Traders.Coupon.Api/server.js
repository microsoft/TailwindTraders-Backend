require("dotenv").config();

const http = require('http');
const mongoose = require('mongoose');
const app = require('./api/app');
const seedData = require('./api/config/seedData');

mongoose.Promise = global.Promise;
mongoose.connect(`mongodb://${process.env.CONNECTIONSTRING}`, { useNewUrlParser: true })
    .then(() => {
        console.log('Connection to CosmosDB successful');
        seedData().then(() => {
            http.createServer(app).listen(process.env.PORT || 3000);
        });
    })
    .catch(err => console.error(err));
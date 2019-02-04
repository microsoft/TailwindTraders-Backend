require('dotenv').config();

const config = {};

config.host = "https://localhost:8081";
config.authKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
config.databaseId = "ShoppingCart";
config.containerId = "Products";

if (config.host.includes("https://localhost:")) {
    console.log("Local environment detected");
    console.log("WARNING: Disabled checking of self-signed certs. Do not have this code in production.");
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
    console.log(`Go to http://localhost:${process.env.PORT || '3000'} to try the sample.`);
}

module.exports = config;
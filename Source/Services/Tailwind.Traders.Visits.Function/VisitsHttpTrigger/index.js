const mongoose = require('mongoose');
const ProductVisitModel = require('./schema/productVisit');
const authorization = require('./security/authorization');

var _connection = (function(){
    var item = process.env.CONNECTIONSTRING.split('/?');
    return [item[0], `/${process.env.DB}?`, item[1]].join('');
})();

mongoose.Promise = global.Promise;
mongoose.connect(_connection, { useNewUrlParser: true })
    .then(() => {
        console.log('Connection to CosmosDB successful');
    })
    .catch(err => console.error(err));

module.exports = async function (context, req) {
    context.log(`VisitsHttpTrigger function processed a request. RequestUri=${req.originalUrl}`);
    context.log(`Request Headers = ${JSON.stringify(req.headers)}`);
    context.log(`Request Body = ${JSON.stringify(req.body)}`);
     
    if(authorization(req, context)){
        if (req.body){
            try{
                let productVisit = new ProductVisitModel({
                    userId: req.body.userId,
                    product: {
                        id: req.body.product.id,
                        name: req.body.product.name,
                        price: req.body.product.price,
                        imageName: req.body.product.imageName,
                        brand: {
                            id: req.body.product.brand.id,
                            name: req.body.product.brand.name
                        },
                        type: {
                            id: req.body.product.type.id,
                            name: req.body.product.type.name
                        },
                        features: req.body.product.features
                    }
                });
    
                productVisit.save(function(err){
                    if(err){
                        context.log.error(err);
                        context.res = {
                            status: 503,
                            body: err
                        };
                    }
                    else{
                        context.log(`Product visit save for user ${req.body.userId}`);
                    }
                });
            }
            catch(ex){
                context.log.error(`${ex.message}`);
                context.res = {
                    status: 400,
                    body: ex.message
                };
            }
        }
        else {
            context.res = {
                status: 400,
                body: `From request ${req.originalUrl}, body is empty`
            };
        }
    }
};
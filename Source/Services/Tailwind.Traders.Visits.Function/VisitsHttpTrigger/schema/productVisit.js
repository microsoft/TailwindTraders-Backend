const mongoose = require('mongoose');

let productVisitSchema = new mongoose.Schema({
    userId: String,
    product: { 
        id: Number,
        name: String,
        price: String,
        imageName: String,
        brand: {
            id: Number,
            name: String
        },
        type: {
            id: Number,
            name: String
        },
        features: [{
            id: Number,
            title: String,
            description: String
        }] 
    }
},
{ 
    collection: process.env.VISITCOLLECTION 
});

module.exports = mongoose.model('ProductVisit', productVisitSchema);
class CartController {
    constructor(shoppingCartDao, recommendedDao) {
        this.shoppingCartDao = shoppingCartDao;
        this.recommendedDao = recommendedDao;
    }

    retrieveUser(req) {
        return req.headers["x-tt-name"];
    }

    // proper fix
    // async addProduct(req, res) {
    //     const item = req.body;
    //     const doc = await this.shoppingCartDao.addItem(item);
    //     res.status(201).send({ message: `${doc.detailProduct.name} added to shopping cart`, id: doc.id });
    //     console.log(`Succsessfull added product ${doc.detailProduct.name} to shopping cart`)
    // }

    // gnome bug
    async addProduct(req, res) {
        // const body = {
        //     detailProduct: {
        //         id: 58,
        //         name: "Single red garden gnome",
        //         price: 56,
        //         imageUrl: "https://github.com/microsoft/TailwindTraders-Backend/blob/main/Deploy/tailwindtraders-images/product-detail/6112251.jpg?raw=true",
        //         email: "admin@tailwindtraders.com",
        //         typeid: 4,
        //     },
        //     qty: 1,
        // };
        const item = req.body;
        const doc = await this.shoppingCartDao.addItem(item);
        res.status(201).send({ message: `${doc.detailProduct.name} added to shopping cart`, id: doc.id });
        console.log(`Succsessfull added product ${doc.detailProduct.name} to shopping cart`)
    }

    async getProductsByUser(req, res) {
        const user = this.retrieveUser(req);
        const items = await this.shoppingCartDao.find(user);
        res.json(items);
    }

    async updateProductQuantity(req, res) {
        const data = req.body;
        if (!data.qty || !data.id) {
            res.status(400).send({ message: "'id' and/or 'qty' missing" });
        }
        else {
            await this.shoppingCartDao.updateQuantity(data.id, data.qty);
            res.status(201).send({ message: "Product qty updated" });
        }
    }

    async deleteItem(req, res) {
        const data = req.body;
        if (!data.id) {
            res.status(400).send({ message: "'id' missing" });
        }
        else {
            await this.shoppingCartDao.deleteItem(data.id);
            res.status(200).send({ message: "Product deleted" });
        }

    }

    async getRelatedProducts(req, res) {
        const user = this.retrieveUser(req);

        const typeid = req.query.type;
        if (!typeid && !user) {
            res.status(400).send({ message: "'user' or 'productType' missing" });
        } else {
            const items = await this.recommendedDao.findRelated(typeid, user);
            res.json(items);
        }
    }

}
module.exports = CartController;
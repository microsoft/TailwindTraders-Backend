class ShoppingCartDao {
  constructor(cosmosClient, databaseId, containerId) {
    this.client = cosmosClient;
    this.databaseId = databaseId;
    this.collectionId = containerId;

    this.database = null;
    this.container = null;
  }

  async init() {
    const dbResponse = await this.client.databases.createIfNotExists({
      id: this.databaseId
    });
    this.database = dbResponse.database;
    const coResponse = await this.database.containers.createIfNotExists({
      id: this.collectionId
    });
    this.container = coResponse.container;
  }

  async find(email) {
    const querySpec = {
      query: "SELECT * FROM r WHERE r.detailProduct.email=@email",
      parameters: [
        {
          name: "@email",
          value: email
        }
      ]
    };

    if (!this.container) {
      throw new Error(`Collection is not initialized. ${this.container}`);
    }

    const { resources: results } = await this.container.items
      .query(querySpec)
      .fetchAll();
    return results.map(i => ({
      id: i.detailProduct.id,
      name: i.detailProduct.name,
      price: i.detailProduct.price,
      imageUrl: i.detailProduct.imageUrl,
      email: i.detailProduct.email,
      qty: i.qty,
      _cdbid: i.id
    }));
  }

  async addItem(item) {
    const { resource: doc } = await this.container.items.create(item);
    return doc;
  }

  async updateQuantity(id, newqty) {
    const itemToReplace = this.container.item(id);
    try {
      const { resource: doc } = await itemToReplace.read();
      doc.qty = newqty;
      await itemToReplace.replace(doc);
    } catch (e) {
      throw new Error(
        `Cosmosdb error ${e.code} when loading doc with id ${id}`
      );
    }
  }

  async deleteItem(id) {
    const itemToDelete = this.container.item(id);
    try {
      await itemToDelete.delete();
    } catch (e) {
      throw new Error(
        `Cosmosdb error ${e.code} when deleting doc with id ${id}`
      );
    }
  }
}

module.exports = ShoppingCartDao;

class RecommendedDao {
    constructor(cosmosClient, databaseId) {
        this.client = cosmosClient;
        this.databaseId = databaseId;
        this.collectionId = 'recommendations';

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


    async findRelated(typeid, email) {
        let typeidToNumber = parseInt(typeid)
        const querySpec = {
            query: "SELECT * FROM r WHERE r.email=@email and r.typeid=@typeid",
            parameters: [
                {
                    name: "@email",
                    value: email,
                },
                {
                    name: "@typeid",
                    value: typeidToNumber,
                }
            ]
        };
        if (!this.container) {
            throw new Error("Collection is not initialized.");
        }
        const { resources: results } = await this.container.items.query(querySpec).fetchAll();
        return results.map(i => ({
            email: i.email,
            typeid: i.typeid,
            recommendations: i.recommendations
        }));
    }
}

module.exports = RecommendedDao;
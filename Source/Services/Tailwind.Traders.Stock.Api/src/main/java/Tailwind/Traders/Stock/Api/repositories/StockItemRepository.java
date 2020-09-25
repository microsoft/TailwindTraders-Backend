package Tailwind.Traders.Stock.Api.repositories;

import java.util.Iterator;
import java.util.UUID;

import com.azure.cosmos.CosmosClient;
import com.azure.cosmos.CosmosContainer;
import com.azure.cosmos.CosmosDatabase;
import com.azure.cosmos.CosmosException;
import com.azure.cosmos.implementation.Document;
import com.azure.cosmos.models.CosmosContainerProperties;
import com.azure.cosmos.models.CosmosDatabaseProperties;
import com.azure.cosmos.models.CosmosItemRequestOptions;
import com.azure.cosmos.models.PartitionKey;
import com.microsoft.applicationinsights.core.dependencies.gson.Gson;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import Tailwind.Traders.Stock.Api.models.StockItem;

@Component
public class StockItemRepository {
	@Value("${azure.cosmosdb.database}")
	private String DATABASE_ID;

	private static final String COLLECTION_ID = "StockCollection";
	private static final String partitonkey = "827bba17-8ee4-4a74-977d-855983431861";
	private static Gson gson = new Gson();

	private static CosmosDatabaseProperties databaseCache;
	private static CosmosContainerProperties containerCache;


	@Autowired
	private CosmosClient cosmosClient;

	public StockItem findByProductId(Integer pid) {
		StockItem stockItemDocument = getItemById(pid);

		return stockItemDocument;
	}

	public void update(StockItem stockItem) {
		CosmosContainer cosmosContainer = cosmosClient.getDatabase(getTodoDatabase().getId()).getContainer(getTodoCollection().getId());
		StockItem stockItemDocument = getItemById(stockItem.getProductId());
		stockItemDocument.setStockCount(stockItem.getStockCount());
		try {
			cosmosContainer.replaceItem(stockItemDocument, stockItemDocument.getId(), new PartitionKey(partitonkey), new CosmosItemRequestOptions());
		} catch (CosmosException e) {
			e.printStackTrace();
		}
	}

	public void save(StockItem stockItem) {
		CosmosContainer cosmosContainer = cosmosClient.getDatabase(getTodoDatabase().getId()).getContainer(getTodoCollection().getId());
		stockItem.setId(UUID.randomUUID().toString());
		stockItem.setPartition(partitonkey);
		Document todoItemDocument = new Document(gson.toJson(stockItem));
		todoItemDocument.set("entityType", "stockItem");
		
		try {
			
			cosmosContainer.createItem(todoItemDocument,new PartitionKey(partitonkey), new CosmosItemRequestOptions());
		} catch (CosmosException e) {
			e.printStackTrace();
		}
	}

	public Integer count() {
		int count= 0;
		CosmosContainer cosmosContainer = cosmosClient.getDatabase(getTodoDatabase().getId()).getContainer(getTodoCollection().getId());
		Iterator<StockItem> allItems = cosmosContainer.readAllItems(new PartitionKey(partitonkey) , StockItem.class).iterator();
		while(allItems.hasNext()){
			allItems.next();
			count++;
		}
		return count;
	}

	private StockItem getItemById(Integer id) {
		CosmosContainer cosmosContainer = cosmosClient.getDatabase(getTodoDatabase().getId()).getContainer(getTodoCollection().getId());
		Iterator<StockItem> documentList = cosmosContainer
				.queryItems("SELECT * FROM root r WHERE r.productId=" + id, null, StockItem.class).iterator();

		if (documentList.hasNext()) {
			return documentList.next();
		}

		return null;
	}

	private CosmosContainerProperties getTodoCollection() {
		if (containerCache != null) {
			return containerCache;
		}
		CosmosDatabase cosmosDatabase = cosmosClient.getDatabase(getTodoDatabase().getId());
		Iterator<CosmosContainerProperties> collectionList = cosmosDatabase
				.queryContainers(
				"SELECT * FROM root r WHERE r.id='" + COLLECTION_ID + "'", null).iterator();

		if (collectionList.hasNext()) {
			containerCache = collectionList.next();
		} else {
			try {
				CosmosContainerProperties cosmosContainerProperties = new CosmosContainerProperties(COLLECTION_ID, "/partition");

				containerCache = cosmosDatabase
						.createContainer(cosmosContainerProperties).getProperties();
			} catch (CosmosException e) {
				e.printStackTrace();
			}
		}

		return containerCache;
	}

	private CosmosDatabaseProperties getTodoDatabase() {
		if (databaseCache != null) {
			return databaseCache;
		}

		Iterator<CosmosDatabaseProperties> databaseList = cosmosClient
				.queryDatabases("SELECT * FROM root r WHERE r.id='" + DATABASE_ID + "'", null).iterator();

		if (databaseList.hasNext()) {
			databaseCache = databaseList.next();
		} else {
			try {
				CosmosDatabaseProperties databaseDefinition = new CosmosDatabaseProperties(DATABASE_ID);
				databaseCache = cosmosClient.createDatabase(databaseDefinition).getProperties();
			} catch (CosmosException e) {
				e.printStackTrace();
			}
		}

        return databaseCache;
    }
}
package Tailwind.Traders.Stock.Api.repositories;

import java.util.List;

import com.microsoft.applicationinsights.core.dependencies.gson.Gson;
import com.microsoft.azure.documentdb.Database;
import com.microsoft.azure.documentdb.Document;
import com.microsoft.azure.documentdb.DocumentClient;
import com.microsoft.azure.documentdb.DocumentClientException;
import com.microsoft.azure.documentdb.DocumentCollection;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import Tailwind.Traders.Stock.Api.models.StockItem;

@Component
public class StockItemRepository
{
    @Value("${azure.cosmosdb.database}")
	private String DATABASE_ID;

	private static final String COLLECTION_ID = "StockCollection";
    private static Gson gson = new Gson();

	private static Database databaseCache;
    private static DocumentCollection collectionCache;
	
	@Autowired
	private DocumentClient documentClient;

	public StockItem findByProductId(Integer pid)
	{
		Document stockItemDocument = getItemById(pid);

        if (stockItemDocument != null) {
            // De-serialize the document in to a StockItem.
            return gson.fromJson(stockItemDocument.toString(), StockItem.class);
        } else {
            return null;
        }
	}

	public void update(StockItem stockItem) {
		Document stockItemDocument = getItemById(stockItem.getProductId());
		stockItemDocument.set("stockCount", stockItem.getStockCount());

        try {
        	documentClient.replaceDocument(stockItemDocument, null);
        } catch (DocumentClientException e) {
            e.printStackTrace();
        }
    }
    
	public void save(StockItem stockItem) {
		Document todoItemDocument = new Document(gson.toJson(stockItem));
        todoItemDocument.set("entityType", "stockItem");

        try {
            todoItemDocument = documentClient.createDocument(getTodoCollection().getSelfLink(), todoItemDocument, null, false).getResource();
        } catch (DocumentClientException e) {
            e.printStackTrace();
        }
	}

	public Integer count()
	{
		List<Document> stockItemDocument = documentClient
			.queryDocuments(getTodoCollection().getSelfLink(), "SELECT 1 FROM root r", null)
			.getQueryIterable().toList();
			
		return stockItemDocument.size();
	}
	
    private Document getItemById(Integer id) {
        List<Document> documentList = documentClient
                .queryDocuments(getTodoCollection().getSelfLink(), "SELECT * FROM root r WHERE r.productId=" + id, null)
                .getQueryIterable().toList();

        if (documentList.size() > 0) {
            return documentList.get(0);
		}
		
		return null;
    }

    private DocumentCollection getTodoCollection() {
		if (collectionCache != null) {
			return collectionCache;
		}

		List<DocumentCollection> collectionList = documentClient
				.queryCollections(
						getTodoDatabase().getSelfLink(),
						"SELECT * FROM root r WHERE r.id='" + COLLECTION_ID + "'",
						null).getQueryIterable().toList();

		if (collectionList.size() > 0) {
			collectionCache = collectionList.get(0);
		} else {
			try {
				DocumentCollection collectionDefinition = new DocumentCollection();
				collectionDefinition.setId(COLLECTION_ID);

				collectionCache = documentClient.createCollection(
						getTodoDatabase().getSelfLink(),
						collectionDefinition, null).getResource();
			} catch (DocumentClientException e) {
				e.printStackTrace();
			}
		}

        return collectionCache;
	}
	
    private Database getTodoDatabase() {
		if (databaseCache != null){
			return databaseCache;
		}

		List<Database> databaseList = documentClient
				.queryDatabases("SELECT * FROM root r WHERE r.id='" + DATABASE_ID + "'", null).getQueryIterable().toList();

		if (databaseList.size() > 0) {
			databaseCache = databaseList.get(0);
		} else {
			try {
				Database databaseDefinition = new Database();
				databaseDefinition.setId(DATABASE_ID);

				databaseCache = documentClient.createDatabase(
						databaseDefinition, null).getResource();
			} catch (DocumentClientException e) {
				e.printStackTrace();
			}
		}

        return databaseCache;
    }
}
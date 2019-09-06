package Tailwind.Traders.Stock.Api.config;

import org.springframework.context.annotation.Bean;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;

import com.microsoft.azure.documentdb.ConnectionPolicy;
import com.microsoft.azure.documentdb.ConsistencyLevel;
import com.microsoft.azure.documentdb.DocumentClient;

@Configuration
public class CosmosDbConfig { 
	//Initialize AI TelemetryConfiguration via Spring Beans
    @Bean
    public DocumentClient cosmosDbConfig(
        @Value("${azure.cosmosdb-auth}") String auth_key, 
        @Value("${azure.cosmosdb-host}") String host) {
            DocumentClient documentClient = new DocumentClient(host, auth_key, ConnectionPolicy.GetDefault(),
                ConsistencyLevel.Session);
            return documentClient;
    }
}

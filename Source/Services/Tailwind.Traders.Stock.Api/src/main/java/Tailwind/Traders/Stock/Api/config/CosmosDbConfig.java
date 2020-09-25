package Tailwind.Traders.Stock.Api.config;

import org.springframework.context.annotation.Bean;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Scope;

import com.azure.cosmos.CosmosClient;
import com.azure.cosmos.CosmosClientBuilder;

@Configuration
public class CosmosDbConfig {
    private final Logger log = LogManager.getLogger(CosmosDbConfig.class);

    @Value("${azure.cosmosdb.auth}")
    private String MASTER_KEY;
    
    @Value("${azure.cosmosdb.host}")
    private String HOST;

    
    @Bean
    @Scope("singleton")
    public CosmosClient cosmosClient() {
            log.info(String.format("CosmosDb Auth key: %s", MASTER_KEY));
            log.info(String.format("CosmosDb host: %s", HOST));
            return new CosmosClientBuilder()
            .endpoint(HOST)
            .key(MASTER_KEY)
            .consistencyLevel(com.azure.cosmos.ConsistencyLevel.SESSION)
            .buildClient();
    }
}

package Tailwind.Traders.Stock.Api.seeders;


import Tailwind.Traders.Stock.Api.StockProduct;
import Tailwind.Traders.Stock.Api.models.StockItem;
import Tailwind.Traders.Stock.Api.repositories.StockItemRepository;
import com.opencsv.bean.CsvToBeanBuilder;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.event.ContextRefreshedEvent;
import org.springframework.context.event.EventListener;
import org.springframework.stereotype.Component;

import java.io.BufferedReader;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;


@Component
public class StockItemSeeder  {
    @Autowired
    private StockItemRepository repository;

    @EventListener
    public void seed(ContextRefreshedEvent event) throws IOException {
        boolean alreadySeeded = repository.count() > 0;

        if (alreadySeeded) {
            return;
        }

        BufferedReader reader = Files.newBufferedReader(Paths.get("setup/StockProduct.csv"), StandardCharsets.UTF_8);
        List<StockProduct> allStock = new CsvToBeanBuilder<StockProduct>(reader).withType(StockProduct.class).build().parse();
        List<Integer> setted = new ArrayList<Integer>();

        for (StockProduct stock : allStock) {
            StockItem item = new StockItem();
            item.setProductId(stock.getId());
            item.setStockCount(stock.getProductStock());
            setted.add(stock.getId());
            repository.save(item);
        }

        // For all other products up to MAX_PRODUCT_ID set a 100 stock units

        String mpid = System.getenv("MAX_PRODUCT_ID");
        int defaultStock=60;
        int maxpid = 0;
        try {
            maxpid = Integer.parseInt(mpid);
        } catch (NumberFormatException ex) {
            maxpid = 250;
        }

        for (int idx=1; idx<=maxpid; idx++) {
            if (!setted.contains(idx)) {
                StockItem item = new StockItem();
                item.setProductId(idx);
                item.setStockCount(defaultStock);
                repository.save(item);
            }
        }
    }
}

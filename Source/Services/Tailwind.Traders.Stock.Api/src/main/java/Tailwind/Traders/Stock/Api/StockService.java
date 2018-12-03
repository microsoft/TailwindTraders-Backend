package Tailwind.Traders.Stock.Api;

import java.io.BufferedReader;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;

import org.springframework.stereotype.Service;

import com.opencsv.bean.CsvToBeanBuilder;

@Service
public class StockService {
	public StockProduct StockById(int productId) throws Exception, IOException {
		BufferedReader reader = Files.newBufferedReader(Paths.get("setup/StockProduct.csv"), StandardCharsets.UTF_8);

		StockProduct stock = new CsvToBeanBuilder<StockProduct>(reader).withType(StockProduct.class).build().parse()
				.stream().filter(item -> item.getId() == productId).findFirst().orElse(null);

		return stock;
	}
}

package Tailwind.Traders.Stock.Api;

import java.io.IOException;

import Tailwind.Traders.Stock.Api.models.StockItem;
import Tailwind.Traders.Stock.Api.repositories.StockItemRepository;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class StockController {
	@Autowired
	private StockItemRepository stockItemRepository;

	private final Logger log = LogManager.getLogger();

	@ResponseBody
	@RequestMapping(value = "/v1/stock/{id}")
	public ResponseEntity<StockProduct> StockProduct(@PathVariable(value="id", required= true) int id) throws IOException, Exception {
		StockItem stock = stockItemRepository.findByProductId(id);

		if (stock == null) {
			log.debug("Not found stock for product " + id);

			return new ResponseEntity<StockProduct>(HttpStatus.NOT_FOUND);
		}

		StockProduct response = new StockProduct();
		response.setId(stock.getProductId());
		response.setProductStock(stock.getStockCount());
		return new ResponseEntity<StockProduct>(response, HttpStatus.OK);
	}

	@ResponseBody
	@PostMapping("/v1/consumptions/stock/{id}")
	public ResponseEntity decreaseStock (@PathVariable int id) {
		StockItem stock = stockItemRepository.findByProductId(id);

		if (stock == null) {
			log.debug("Not found stock for product " + id);
			return new ResponseEntity<StockProduct>(HttpStatus.NOT_FOUND);
		}

		int currentStock = stock.getStockCount();
		if (currentStock > 0) {
			stock.setStockCount(currentStock - 1);
			stockItemRepository.save(stock);
			return new ResponseEntity(stock, HttpStatus.OK);
		}

		return new ResponseEntity(HttpStatus.BAD_REQUEST);

	}
}

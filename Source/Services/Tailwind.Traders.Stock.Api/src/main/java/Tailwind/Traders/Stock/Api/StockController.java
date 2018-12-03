package Tailwind.Traders.Stock.Api;

import java.io.IOException;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class StockController {
	@Autowired
	private StockService stockService;
	private final Logger log = LogManager.getLogger();

	@ResponseBody
	@RequestMapping(value = "/v1/stock/{id}")
	public ResponseEntity<StockProduct> StockProduct(@PathVariable(value="id", required= true) int id) throws IOException, Exception {
		StockProduct stock = stockService.StockById(id);

		if (stock == null) {
			log.debug("Not found stock for product " + id);

			return new ResponseEntity<StockProduct>(HttpStatus.NOT_FOUND);
		}

		return new ResponseEntity<StockProduct>(stock, HttpStatus.OK);
	}
}

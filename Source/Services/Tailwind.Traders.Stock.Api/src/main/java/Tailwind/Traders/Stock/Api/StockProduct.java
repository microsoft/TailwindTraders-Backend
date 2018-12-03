package Tailwind.Traders.Stock.Api;

import com.opencsv.bean.CsvBindByName;

public class StockProduct {
	@CsvBindByName
	private int id;

	@CsvBindByName
	private int productStock;

	public StockProduct() {
	}

	public StockProduct(int id, int productStock) {
		this.id = id;
		this.productStock = productStock;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getProductStock() {
		return productStock;
	}

	public void setProductStock(int productStock) {
		this.productStock = productStock;
	}
}

package Tailwind.Traders.Stock.Api;

import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;

public class StockItem {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    private Integer id;

    private Integer productId;

    private Integer stockCount;

    public Integer getId() {
        return this.id;
    }
    public void setId(Integer id) {
        this.id = id;
    }

    public Integer getProductId() {
        return this.productId;
    }
    public void setProductId(Integer pid) {
        this.productId = pid;
    }

    public Integer getStockCount {return this.stockCount; }
    public void setStockCount(Integer sc) { this.stockCount = sc; }
}

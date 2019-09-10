package Tailwind.Traders.Stock.Api.models;

import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;

@Entity
public class StockItem {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    private String id;

    private Integer productId;

    private Integer stockCount;

    public String getId() {
        return this.id;
    }
    public void setId(String id) {
        this.id = id;
    }

    public Integer getProductId() {
        return this.productId;
    }
    public void setProductId(Integer pid) {
        this.productId = pid;
    }

    public Integer getStockCount() {return this.stockCount; }
    public void setStockCount(Integer sc) { this.stockCount = sc; }
}

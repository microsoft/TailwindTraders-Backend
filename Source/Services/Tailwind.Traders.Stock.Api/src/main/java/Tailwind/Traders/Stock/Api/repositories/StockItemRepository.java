package Tailwind.Traders.Stock.Api.repositories;

import Tailwind.Traders.Stock.Api.models.StockItem;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.repository.JpaRepository;

public interface StockItemRepository extends JpaRepository<StockItem, Integer> {
    StockItem findByProductId(Integer pid);
}


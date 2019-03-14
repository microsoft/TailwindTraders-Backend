package db

import (
	models "github.com/you/tailwindtraderspopularproducts/models"
)

func GetProducts() ([]models.Product, error) {
	var products []models.Product

	// TODO: Check azure sql issue when trying to seed data

	/*
		    db, ctx := CreateConnection()

		    tsql := fmt.Sprintf("SELECT * FROM dbo.Products;")

		    // Execute query
		    rows, err := db.QueryContext(ctx, tsql)
		    if err != nil {
		        return products, err
		    }

		    defer rows.Close()
		    defer db.Close()

		    for rows.Next() {
				var name, imagename string
				var price float32
				var id int

		        // Get values from row.
		        err := rows.Scan(&id, &name, &imagename, &price)
		        if err != nil {
		            return products, err
				}
				products = append(products, models.Product{ ID: id, Name: name, Price: price, ImageName: imagename })
		    }
	*/

	// Mock result
	p1 := models.Product{
		ID:        1,
		Name:      "Microwave 0.9 Cu. Ft. 900 W",
		Price:     100,
		ImageName: "10446729.jpg",
	}
	p2 := models.Product{
		ID:        3,
		Name:      "Oven 900 W",
		Price:     300,
		ImageName: "26881473.jpg",
	}
	p3 := models.Product{
		ID:        7,
		Name:      "Refrigerator ft. 90 watts",
		Price:     400,
		ImageName: "45808024.jpg",
	}

	products = append(products, p1)
	products = append(products, p2)
	products = append(products, p3)

	return products, nil
}

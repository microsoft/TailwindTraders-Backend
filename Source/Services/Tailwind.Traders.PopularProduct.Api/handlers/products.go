package handler

import (
	"encoding/json"
	"net/http"
	"github.com/you/tailwindtraderspopularproducts/db"
	"log"
	dtos "github.com/you/tailwindtraderspopularproducts/dtos"
	"fmt"
    "github.com/you/tailwindtraderspopularproducts/config"
)

// GET /products
func GetProducts(w http.ResponseWriter, r *http.Request) {
	var result []dtos.ProductDTO
	configValues := configuration.Values
	azureStorageUrl := configValues.AzureStorageUrl

	products,err := db.GetProducts()

	if err != nil {
		log.Fatal("Error retrieving Products from DB: ", err.Error())
	}

	for _, value := range products {
  		result = append(
			result, 
			dtos.ProductDTO { 
				ID: value.ID, 
				Name: value.Name, 
				Price: value.Price, 
				ImageUrl: fmt.Sprintf("%s/%s", azureStorageUrl, value.ImageName), 
			},
		)
	}
		
	w.Header().Set("Content-Type", "application/json; charset=UTF-8")
	w.WriteHeader(http.StatusOK)
	json.NewEncoder(w).Encode(result)
}
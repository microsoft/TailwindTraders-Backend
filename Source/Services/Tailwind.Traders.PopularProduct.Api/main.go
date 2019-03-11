package main

import (
	
    configuration "github.com/you/tailwindtraderspopularproducts/config"
	"github.com/you/tailwindtraderspopularproducts/middlewares"
	"github.com/you/tailwindtraderspopularproducts/router"
	"log"
	"net/http"
)

func main() {

	// Load app configuration
	configuration.Load()

	// Seed data
	// db.Seed()

	// create router and start listen on port 80
	router := router.NewRouter()
	log.Fatal(http.ListenAndServe(":80", middlewares.SetupGlobalMiddlewares(router)))
}

package main

import (
	configuration "app/config"
	"app/middlewares"
	"app/router"
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

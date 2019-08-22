package main

import (
	
    configuration "github.com/you/tailwindtraderspopularproducts/config"
	"github.com/you/tailwindtraderspopularproducts/middlewares"
	"github.com/you/tailwindtraderspopularproducts/router"
	"github.com/microsoft/ApplicationInsights-Go/appinsights"
	"log"
	"net/http"
)

func main() {

	// Load app configuration
	configuration.Load()

	ApplicationInsights__InstrumentationKey := config.ApplicationInsights__InstrumentationKey
	
	if ApplicationInsights__InstrumentationKey != "" { 	
		client := appinsights.NewTelemetryClient(ApplicationInsights__InstrumentationKey) 
	}

	// Seed data
	// db.Seed()

	// create router and start listen on port 80
	router := router.NewRouter()
	log.Fatal(http.ListenAndServe(":80", middlewares.SetupGlobalMiddlewares(router)))
}

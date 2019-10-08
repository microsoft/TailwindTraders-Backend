package main

import (
	"log"
	"net/http"

	"github.com/microsoft/ApplicationInsights-Go/appinsights"
	configuration "github.com/you/tailwindtraderspopularproducts/config"
	"github.com/you/tailwindtraderspopularproducts/middlewares"
	"github.com/you/tailwindtraderspopularproducts/router"
)

func main() {

	// Load app configuration
	configuration.Load()

	ApplicationInsightsInstrumentationKey := configuration.Values.ApplicationInsightsIK

	if ApplicationInsightsInstrumentationKey != "" {
		appinsights.NewTelemetryClient(ApplicationInsightsInstrumentationKey)
	}

	// Seed data
	// db.Seed()

	// create router and start listen on port 80
	router := router.NewRouter()
	log.Fatal(http.ListenAndServe(":80", middlewares.SetupGlobalMiddlewares(router)))
}

package main

import (
    "log"
    "net/http"
    "app/router"
    "app/middlewares"
    "app/config"
)

func main() {
    
    // Load app configuration
    configuration.Load()

    // Seed data
    //db.Seed()
    
    // create router and start listen on port 80
    router := router.NewRouter()
    log.Fatal(http.ListenAndServe(":80", middlewares.SetupGlobalMiddlewares(router)))
}
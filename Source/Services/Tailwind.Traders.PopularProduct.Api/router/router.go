package router

import (
	"github.com/gorilla/mux"
	"github.com/you/tailwindtraderspopularproducts/middlewares"
)

func NewRouter() *mux.Router {
	router := mux.NewRouter().StrictSlash(true)
	router.Use(middlewares.Logger)
	sub := router.PathPrefix("/v1").Subrouter()

	for _, route := range routes {
		sub.
			HandleFunc(route.Pattern, route.HandlerFunc).
			Name(route.Name).
			Methods(route.Method)
	}

	return router
}
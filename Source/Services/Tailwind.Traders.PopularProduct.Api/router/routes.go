package router

import (
	"net/http"

	handler "github.com/you/tailwindtraderspopularproducts/handlers"
)

type Route struct {
	Name        string
	Method      string
	Pattern     string
	HandlerFunc http.HandlerFunc
}

type Routes []Route

var routes = Routes{
	Route{
		"GetProducts",
		"GET",
		"/products",
		handler.GetProducts,
	},
}
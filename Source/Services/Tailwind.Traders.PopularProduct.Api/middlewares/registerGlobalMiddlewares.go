package middlewares

import (
	"net/http"
	"github.com/rs/cors"
)

func SetupGlobalMiddlewares(handler http.Handler) http.Handler {
	corsHandler := cors.Default().Handler
	authHandler := AuthenticationMiddleware
	handler = corsHandler(handler)
	handler = authHandler(handler)

	return handler;
}

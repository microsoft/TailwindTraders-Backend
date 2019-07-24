package middlewares

import (
	"net/http"

	"github.com/rs/cors"
)

func SetupGlobalMiddlewares(handler http.Handler) http.Handler {
	corsHandler := cors.Default().Handler
	healthCheckHandler := healthCheckMiddleware
	handler = corsHandler(handler)
	handler = healthCheckHandler(handler)
	return handler
}

package middlewares

import (
	"net/http"
	"github.com/rs/cors"
)

func SetupGlobalMiddlewares(handler http.Handler) http.Handler {
	corsHandler := cors.Default().Handler
	handler = corsHandler(handler)

	return handler;
}

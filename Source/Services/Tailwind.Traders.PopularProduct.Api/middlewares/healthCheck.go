package middlewares

import "net/http"

func healthCheckMiddleware(inner http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		if r.URL.Path == "/liveness" {
			w.Write([]byte("Healthy"))
			return
		}
		inner.ServeHTTP(w, r)
	})
}

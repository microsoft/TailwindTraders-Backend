package middlewares

import(
	"log"
	"net/http"
	"strings"
)

func AuthenticationMiddleware(next http.Handler) http.Handler {
    return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
        token := r.Header.Get("Authorization")

        if value := token; strings.Contains(value, "Email ") {
			user := strings.Split(token, " ")[1]
        	log.Printf("Authenticated user %s\n", user)
        	// Pass down the request to the next middleware (or final handler)
        	next.ServeHTTP(w, r)
        } else {
        	// Write an error and stop the handler chain
        	http.Error(w, "Unauthorized", http.StatusUnauthorized)
        }
    })
}
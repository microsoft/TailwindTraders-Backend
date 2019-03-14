package middlewares

import (
	configuration "github.com/you/tailwindtraderspopularproducts/config"
	"net/http"
	"strings"

	"github.com/gbrlsnchs/jwt/v3"
)

//AuthenticationMiddleware token
func AuthenticationMiddleware(next http.Handler) http.Handler {

	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {

		config := configuration.Values
		securityKey := config.SecurityKey
		issuer := config.Issuer

		type Token struct {
			*jwt.Payload
			IsLoggedIn bool   `json:"isLoggedIn"`
			Issuer     string `json:"customField,omitempty"`
		}

		if len(r.Header.Get("Authorization")) == 0 {
			http.Error(w, "Auth token is not supplied", http.StatusUnauthorized)
			return
		}

		reqToken := r.Header.Get("Authorization")
		splitToken := strings.Split(reqToken, "Bearer ")
		reqToken = splitToken[1]
		token := []byte(reqToken)

		hs256 := jwt.NewHMAC(jwt.SHA256, []byte(securityKey))

		raw, err := jwt.Parse(token)

		if err != nil {
			http.Error(w, err.Error(), http.StatusUnauthorized)
			return
		}
		if err = raw.Verify(hs256); err != nil {
			http.Error(w, err.Error(), http.StatusUnauthorized)
			return
		}

		var (
			_ jwt.Header
			p Token
		)

		if _, err = raw.Decode(&p); err != nil {
			http.Error(w, err.Error(), http.StatusUnauthorized)
				return
		}

		issValidator := jwt.IssuerValidator(issuer)

		if err := p.Validate(issValidator); err != nil {
			switch err {
			case jwt.ErrIssValidation:
				http.Error(w, err.Error(), http.StatusUnauthorized)
				return
			}
		}

		next.ServeHTTP(w, r)
	})
}

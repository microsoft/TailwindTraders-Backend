package middlewares

import (
	configuration "app/config"
	"net/http"
	"strings"

	"github.com/gbrlsnchs/jwt"
)

//AuthenticationMiddleware token
func AuthenticationMiddleware(next http.Handler) http.Handler {

	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {

		config := configuration.Values
		securityKey := config.SecurityKey
		issuer := config.Issuer

		type Token struct {
			*jwt.JWT
			IsLoggedIn bool   `json:"isLoggedIn"`
			Issuer     string `json:"customField,omitempty"`
		}

		if len(r.Header.Get("Authorization")) == 0 {
			http.Error(w, "Auth token is not supplied", http.StatusBadRequest)
			return
		}

		reqToken := r.Header.Get("Authorization")
		splitToken := strings.Split(reqToken, "Bearer ")
		reqToken = splitToken[1]

		hs256 := jwt.NewHS256(securityKey)

		payload, sig, err := jwt.Parse(reqToken)

		if err != nil {
			http.Error(w, err.Error(), http.StatusUnauthorized)
			return
		}
		if err = hs256.Verify(payload, sig); err != nil {
			http.Error(w, err.Error(), http.StatusUnauthorized)
			return
		}

		jot := &jwt.JWT{
			Issuer: issuer,
		}
		if err = jwt.Unmarshal(payload, &jot); err != nil {
			http.Error(w, err.Error(), http.StatusUnauthorized)
			return
		}

		issValidator := jwt.IssuerValidator(issuer)
		if err = jot.Validate(issValidator); err != nil {
			switch err {
			case jwt.ErrIssValidation:
				http.Error(w, err.Error(), http.StatusUnauthorized)
				return
			}
		}

		next.ServeHTTP(w, r)
	})
}

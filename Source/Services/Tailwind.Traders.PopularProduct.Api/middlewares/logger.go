package middlewares

import (
	"log"
	"net/http"
	"time"
)

type LoggingResponseWriter struct {
	http.ResponseWriter
	statusCode int
}

func ResponseWriterWrapper(w http.ResponseWriter) *LoggingResponseWriter {
	return &LoggingResponseWriter{w, http.StatusOK}
}

func (lrw *LoggingResponseWriter) WriteHeader(code int) {
	lrw.statusCode = code
	lrw.ResponseWriter.WriteHeader(code)
}

func Logger(inner http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		start := time.Now()
		wrapper := ResponseWriterWrapper(w)
		inner.ServeHTTP(wrapper, r)
		log.Printf("%s %s %s [%v] \"%s %s %s\" %d %d \"%s\" %s",
			r.RemoteAddr,
			"-",
			"-",
			start,
			r.Method,
			r.RequestURI,
			r.Proto, 
			wrapper.statusCode,
			r.ContentLength,
			r.Header["User-Agent"],
			time.Since(start),
		)
	})
}
package db

import (
    _ "github.com/denisenkom/go-mssqldb"
    "database/sql"
    "context"
    "log"
	"fmt"
    "github.com/you/tailwindtraderspopularproducts/config"
)

func CreateConnection() (*sql.DB, context.Context) {
	var err error
	config := configuration.Values
	connString := config.ConnectionString
	// Create connection pool
    db, err := sql.Open("sqlserver", connString)
    if err != nil {
        log.Fatal("Error creating connection pool: ", err.Error())
    }
    ctx := context.Background()
    err = db.PingContext(ctx)
    if err != nil {
        log.Fatal(err.Error())
    }

	fmt.Printf("Connected!\n")
	
	return db, ctx
}
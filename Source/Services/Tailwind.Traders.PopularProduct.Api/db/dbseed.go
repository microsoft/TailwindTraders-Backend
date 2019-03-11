package db

import (
	"github.com/you/tailwindtraderspopularproducts/csv"
	"database/sql"
	"strconv"

	_ "github.com/denisenkom/go-mssqldb"
)

func Seed() {
	db, ctx := CreateConnection()
	data := csv.LoadCsvData()

	for _, line := range data {
		tsql := "INSERT INTO [dbo].[Products]([Id],[Name],[ImageName],[Price]) VALUES(@Id, @Name, @ImageName, @Price);"

		stmt, err := db.Prepare(tsql)
		if err != nil {
			panic(err)
		}
		defer stmt.Close()
		defer db.Close()

		id, err := strconv.ParseInt(line[0], 0, 64)
		name := line[1]
		imageName := line[2]
		price, err := strconv.ParseFloat(line[3], 32)
		if err != nil {
			panic(err)
		}

		_ = stmt.QueryRowContext(
			ctx,
			sql.Named("Id", id),
			sql.Named("Name", name),
			sql.Named("ImageName", imageName),
			sql.Named("Price", float32(price)))
	}
}

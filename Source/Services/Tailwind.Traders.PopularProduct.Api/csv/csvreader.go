package csv

import (
	"encoding/csv"
	"os"
	"path/filepath"
)

func LoadCsvData() [][]string {
	filePath, _  := filepath.Abs("setup/Products.csv")

	f, err := os.Open(filePath)
	if err != nil {
		panic(err)
	}
	defer f.Close()

	lines, err := csv.NewReader(f).ReadAll()
	if err != nil {
		panic(err)
	}

	return lines[1:]
}
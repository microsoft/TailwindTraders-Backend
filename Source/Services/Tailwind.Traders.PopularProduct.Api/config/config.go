package configuration

import (
	"path/filepath"

	"github.com/tkanos/gonfig"
)

var Values Configuration

type Configuration struct {
	ConnectionString string
	SecurityKey      string
	Issuer           string
	AzureStorageUrl  string
	ApplicationInsightsIK string
}

func Load() {
	configuration := Configuration{}
	filePath, _ := filepath.Abs("config/appsettings.json")
	err := gonfig.GetConf(filePath, &configuration)
	if err != nil {
		panic(err)
	}
	Values = configuration
}

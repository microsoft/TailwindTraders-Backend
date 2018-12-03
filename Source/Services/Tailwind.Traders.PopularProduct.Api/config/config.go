package configuration

import (
        "github.com/tkanos/gonfig"
        "path/filepath"
)

var Values Configuration

type Configuration struct {
        ConnectionString string
        AzureStorageUrl string
}

func Load() {
        configuration := Configuration{}
        filePath, _  := filepath.Abs("config/appsettings.json")
        err := gonfig.GetConf(filePath, &configuration)
        if err != nil {
                panic(err)
        }
        Values = configuration
}
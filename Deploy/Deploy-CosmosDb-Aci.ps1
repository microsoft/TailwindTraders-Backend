Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$name
)

Write-Host "Creating ACI for running the CosmosDb Emulator on RG $resourceGroup"

az container create -n $name --image tailwindtraders/cosmosdb_emulator:latest -g $resourceGroup --dns-name-label $name  --ports 8081 --os-type Windows

Write-Host "CosmosDb Emulator deployed on ACI $resourceGroup/$name" -ForegroundColor Yellow
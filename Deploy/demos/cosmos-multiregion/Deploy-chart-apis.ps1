Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$region1,
    [parameter(Mandatory=$true)][string]$region2,
    [parameter(Mandatory=$true)][string]$acrName,
    [parameter(Mandatory=$true)][string]$webappNamePrefix,
    [parameter(Mandatory=$true)][string]$cartCosmosDbName,
    [parameter(Mandatory=$false)][string]$servicePlanSku="S1",
    [parameter(Mandatory=$false)][string]$imageTag="latest"
)

$acrLoginServer=$(az acr show -g $resourceGroup -n $acrName -o json | ConvertFrom-Json).loginServer
$acrCredentials=$(az acr credential show -g $resourceGroup -n $acrName -o json | ConvertFrom-Json)
$cosmosCredentials=$(az cosmosdb list-keys -g $resourceGroup -n $cartCosmosDbName -o json | ConvertFrom-Json).primaryMasterKey
$cosmos=$(az cosmosdb show -g $resourceGroup -n $cartCosmosDbName -o json | ConvertFrom-Json)
$cosmosEndpoint=$cosmos.documentEndpoint

Write-Host "Creating service plan for chart api 1" -ForegroundColor Yellow
az appservice plan create --name ${webappNamePrefix}-splan1 --resourceGroup $resourceGroup --sku $servicePlanSku --is-linux

Write-Host "Creating service plan for chart api 1" -ForegroundColor Yellow
az appservice plan create --name ${webappNamePrefix}-splan2 --resourceGroup $resourceGroup --sku $servicePlanSku --is-linux

Write-Host "Creating webapp for chart api1"
az webapp plan create --name "$webappNamePrefix-1" --plan ${webappNamePrefix}-splan1 --resourceGroup $resourceGroup -deployment-container-image-name nginx
az webapp config container set -g $resourceGroup -n "$webappNamePrefix-1" --docker-registry-server-url https://$acrLoginServer --docker-custom-image-name $acrLoginServer/cart.api:${imageTag}
az webapp config appsettings set -g $resourceGroup -n "$webappNamePrefix-1" --settings AUTHKEY=$cosmosCredentials HOST=$cosmosEndpoint ISSUER=TTFakeLogin SECURITYKEY=nEpLzQJGNSCNL5H6DIQCtTdNxf5VgAGcBbtXLms1YDD01KJBAs0WVawaEjn97uwB


Write-Host "Creating webapp for chart api2"
az webapp plan create --name "$webappNamePrefix-2" --plan ${webappNamePrefix}-splan2 --resourceGroup $resourceGroup -deployment-container-image-name nginx
az webapp config container set -g $resourceGroup -n "$webappNamePrefix-2" --docker-registry-server-url https://$acrLoginServer --docker-custom-image-name $acrLoginServer/cart.api:${imageTag}




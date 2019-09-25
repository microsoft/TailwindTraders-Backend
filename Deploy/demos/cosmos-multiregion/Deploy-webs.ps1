Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$region1,
    [parameter(Mandatory=$true)][string]$region2,
    [parameter(Mandatory=$true)][string]$acrName,
    [parameter(Mandatory=$true)][string]$apiNamePrefix,
    [parameter(Mandatory=$true)][string]$webappNamePrefix,
    [parameter(Mandatory=$true)][string]$apiurl,
    [parameter(Mandatory=$false)][string]$servicePlanSku="S1",
    [parameter(Mandatory=$false)][string]$imageTag="latest"
)

$acrLoginServer=$(az acr show -g $resourceGroup -n $acrName -o json | ConvertFrom-Json).loginServer
$acrCredentials=$(az acr credential show -g $resourceGroup -n $acrName -o json | ConvertFrom-Json)

Write-Host "Creating service plan for web 1 in $region1" -ForegroundColor Yellow
#az appservice plan create --name ${webappNamePrefix}-splan1 -g $resourceGroup --sku $servicePlanSku --location $region1 --is-linux

Write-Host "Creating service plan for web 2 in $region2" -ForegroundColor Yellow
#az appservice plan create --name ${webappNamePrefix}-splan2 -g $resourceGroup --sku $servicePlanSku --location $region2 --is-linux

Write-Host "Creating webapp for web1 in $region1"
#az webapp  create  --name "$webappNamePrefix-$region1" --plan ${webappNamePrefix}-splan1 -g $resourceGroup --deployment-container-image-name nginx
#az webapp config container set -g $resourceGroup -n "$webappNamePrefix-$region1" --docker-registry-server-url https://$acrLoginServer --docker-custom-image-name $acrLoginServer/web:${imageTag}

Write-Host "Creating webapp for web2 in $region2"
#az webapp  create --name "$webappNamePrefix-$region2" --plan ${webappNamePrefix}-splan2 -g $resourceGroup --deployment-container-image-name nginx
#az webapp config container set -g $resourceGroup -n "$webappNamePrefix-$region2" --docker-registry-server-url https://$acrLoginServer --docker-custom-image-name $acrLoginServer/web:${imageTag}

$api1=$(az webapp show  -g $resourceGroup --name "$apiNamePrefix-$region1" -o json | ConvertFrom-Json)
$api2=$(az webapp show -g $resourceGroup --name "$apiNamePrefix-$region2" -o json | ConvertFrom-Json)

$api1HostName=$api1.hostNames[0]
$api2HostName=$api2.hostNames[0]

 
Write-Host "Setting configuration of web1 to use $api1HostName for cart API" -ForegroundColor Yellow
az webapp config appsettings set -g $resourceGroup -n "$webappNamePrefix-$region1" --settings ApiUrlShoppingCart="https://$api1HostName" ApiUrl=$apiurl

Write-Host "Setting configuration of web2 to use $api2HostName for cart API" -ForegroundColor Yellow
az webapp config appsettings set -g $resourceGroup -n "$webappNamePrefix-$region2" --settings ApiUrlShoppingCart="https://$api2HostName" ApiUrl=$apiurl

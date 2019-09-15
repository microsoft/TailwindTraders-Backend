Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$aksName
)

$url = $(az aks show -n $aksName -g $resourceGroup -o json --query addonProfiles.httpapplicationrouting.config.HTTPApplicationRoutingZoneName | ConvertFrom-Json)

if (-not $url) {
    $url=$(az aks show -n $aksName -g $resourceGroup -o json --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName | ConvertFrom-Json)
}

if (-not $url) {
    Write-Host "AKS $aksName not found in RG $resourceGroup or do not have HttpApplicationRouting enabled" -ForegroundColor Red
    exit 1
}

$constr = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://$url/blobs/devstoreaccount1;"

Write-Host "Connecting to Azurite running in AKS $aksName using $constr and creating containers" -ForegroundColor Green
az storage container create --name "coupon-list" --public-access blob --connection-string "$constr" 
az storage container create --name "product-detail" --public-access blob --connection-string "$constr"
az storage container create --name "product-list" --public-access blob --connection-string "$constr"
az storage container create --name "profiles-list" --public-access blob --connection-string "$constr"
Write-Host "Copying images..." -ForegroundColor Green

$accountName=$storage.name
az storage blob upload-batch --connection-string "$constr" --destination coupon-list  --source .\tt-images\coupon-list 
az storage blob upload-batch --connection-string "$constr" --destination product-detail --source .\tt-images\product-detail 
az storage blob upload-batch --connection-string "$constr" --destination product-list --source .\tt-images\product-list 
az storage blob upload-batch --connection-string "$constr" --destination profiles-list --source .\tt-images\profiles-list 









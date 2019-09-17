Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$true)][string]$subscription,
    [parameter(Mandatory=$false)][string]$clientId,
    [parameter(Mandatory=$false)][string]$password,
    [parameter(Mandatory=$false)][bool]$deployAks=$true
)

## Deploy ARM
& ((Split-Path $MyInvocation.InvocationName) + "\Deploy-Arm-Azure.ps1") -resourceGroup $resourceGroup -location $location -clientId $clientId -password $password -deployAks $deployAks

## Connecting kubectl to AKS
Write-Host "Login in your account" -ForegroundColor Yellow
az login

Write-Host "Choosing your subscription" -ForegroundColor Yellow
az account list -o table
az account set --subscription $subscription

Write-Host "Retrieving Aks Name" -ForegroundColor Yellow
$aksName = $(az resource list --resource-group $resourceGroup --resource-type Microsoft.ContainerService/managedClusters -o json | ConvertFrom-Json).name
Write-Host "The name of your AKS: $aksName" -ForegroundColor Yellow

Write-Host "Retrieving credentials" -ForegroundColor Yellow
az aks get-credentials -n $aksName -g $resourceGroup
kubectl config current-context

# ## Add Tiller
& ((Split-Path $MyInvocation.InvocationName) + "\Add-Tiller.ps1")

# ## Generate Config
& ((Split-Path $MyInvocation.InvocationName) + "\Generate-Config.ps1") -resourceGroup $resourceGroup -outputFile helm\__values\configFile.yaml

## Create Secrets
$acrName = $(az acr list --resource-group $resourceGroup --subscription $subscription -o json | ConvertFrom-Json).name
Write-Host "The Name of your ACR: $acrName" -ForegroundColor Yellow
& ((Split-Path $MyInvocation.InvocationName) + "\Create-Secret.ps1") -resourceGroup $resourceGroup -acrName $acrName

# Build an Push
& ((Split-Path $MyInvocation.InvocationName) + "\Build-Push.ps1") -resourceGroup $resourceGroup -acrName $acrName -isWindows $false

# Deploy images in AKS
& ((Split-Path $MyInvocation.InvocationName) + "\Deploy-Images-Aks.ps1") -aksName $aksName -resourceGroup $resourceGroup -charts "*" -acrName $acrName -valuesFile __values\configFile.yaml

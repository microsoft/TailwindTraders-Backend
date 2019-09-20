Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$true)][string]$subscription,
    [parameter(Mandatory=$false)][string]$clientId,
    [parameter(Mandatory=$false)][string]$password,
    [parameter(Mandatory=$true)][string]$rewardsResourceGroup,
    [parameter(Mandatory=$true)][string]$rewardsDbPassword,
    [parameter(Mandatory=$true)][string]$csprojPath,
    [parameter(Mandatory=$false)][string]$msBuildPath="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin",
    [parameter(Mandatory=$false)][bool]$deployAks=$true
)
$gValuesFile="configFile.yaml"

Push-Location $($MyInvocation.InvocationName | Split-Path)

# Update the extension to make sure you have the latest version installed
az extension add --name aks-preview
az extension update --name aks-preview

## Deploy ARM WINDOWS!
.\Deploy-Arm-Azure.ps1 -resourceGroup $resourceGroup -location $location -clientId $clientId -password $password -deployAks $deployAks -deployWinLinux $true

# Connecting kubectl to AKS
Write-Host "Login in your account" -ForegroundColor Yellow
az login

Write-Host "Choosing your subscription" -ForegroundColor Yellow
az account set --subscription $subscription

Write-Host "Retrieving Aks Name" -ForegroundColor Yellow
$aksName = $(az aks list -g $resourceGroup -o json | ConvertFrom-Json).name
Write-Host "The name of your AKS: $aksName" -ForegroundColor Yellow

Write-Host "Retrieving credentials" -ForegroundColor Yellow
az aks get-credentials -n $aksName -g $resourceGroup

# ## Add Tiller
.\Add-Tiller.ps1

## Generate Config
.\Generate-Config.ps1 -resourceGroup $resourceGroup -outputFile "helm\__values\$gValuesFile" -rewardsResourceGroup $rewardsResourceGroup -rewardsDbPassword $rewardsDbPassword

## Create Secrets
$acrName = $(az acr list --resource-group $resourceGroup --subscription $subscription -o json | ConvertFrom-Json).name
Write-Host "The Name of your ACR: $acrName" -ForegroundColor Yellow
.\Create-Secret.ps1 -resourceGroup $resourceGroup -acrName $acrName

# Change to Windows 
$pathDockerCli = $(Get-Item $(Get-Command docker).Path).Directory.Parent.Parent.FullName
$pathDockerCli = "$pathDockerCli\DockerCli.exe"
Write-Host "DockerCli path: $pathDockerCli" -ForegroundColor Yellow

& $pathDockerCli -SwitchWindowsEngine

# MsBuild WCF Services
Push-Location $msBuildPath
$apiproj = $csprojPath
./MSBuild.exe "$apiproj" /p:DeployOnBuild=true /p:PublishProfile=FolderProfile.pubxml

Pop-Location

# Build and Push WCF 
.\Build-Push.ps1 -resourceGroup $resourceGroup -acrName $acrName -isWindows $true

# Deploy images in AKS
.\Deploy-Images-Aks.ps1 -aksName $aksName -resourceGroup $resourceGroup -charts "rr" -acrName $acrName -valuesFile "__values\$gValuesFile"

# Change to Linux 
& $pathDockerCli -SwitchLinuxEngine

# Build and Push
.\Build-Push.ps1 -resourceGroup $resourceGroup -acrName $acrName -isWindows $false

# Deploy images in AKS
.\Deploy-Images-Aks.ps1 -aksName $aksName -resourceGroup $resourceGroup -charts "*" -acrName $acrName -valuesFile "__values\$gValuesFile"

# Deploy pictures in AKS
$storageName = $(az resource list --resource-group $resourceGroup --resource-type Microsoft.Storage/storageAccounts -o json | ConvertFrom-Json).name
.\Deploy-Pictures-Azure.ps1 -resourceGroup $resourceGroup -storageName $storageName

Pop-Location



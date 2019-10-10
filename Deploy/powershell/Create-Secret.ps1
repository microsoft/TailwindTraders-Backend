#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$false)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$acrName,
    [parameter(Mandatory=$false)][string]$clientId,
    [parameter(Mandatory=$false)][string]$password,
    [parameter(Mandatory=$false)][string]$namespace=""
)

function validate {
    $valid=$true

    if ([string]::IsNullOrEmpty($resourceGroup)) {
        Write-Host "No resource group. Use -resourceGroup to specify resource group." -ForegroundColor Red
        $valid=$false
    }

    if ([string]::IsNullOrEmpty($acrLogin)) {
        Write-Host " ACR login server can't be found. Are you using right ACR ($acrName) and RG ($resourceGroup)?" -ForegroundColor Red
        $valid=$false
    }
    
    if ([string]::IsNullOrEmpty($clientId) -and -not [string]::IsNullOrEmpty($password) ) {
        Write-Host "No Client ID. Use -clientid to specify a Client ID if -password is used" -ForegroundColor Red
        Write-Host "If want to use ACR credentials instead of service principal do not pass -clientid NOR -password" -ForegroundColor Red
        $valid=$false
    }
    if ([string]::IsNullOrEmpty($password) -and -not [string]::IsNullOrEmpty($clientId)  ) {
    
        Write-Host "No Client ID Pwd. Use -password to specify a Client ID Password if -clientid is used" -ForegroundColor Red
        Write-Host "If want to use ACR credentials instead of service principal do not pass -clientid NOR -password" -ForegroundColor Red
        $valid=$false
    }
    if ($valid -eq $false) {
        exit 1
    }
}

function createSecret($_clientId, $_password) {
    if ([string]::IsNullOrEmpty($namespace)) {
        kubectl delete secret acr-auth
        kubectl create secret docker-registry acr-auth --docker-server $acrLogin --docker-username $_clientId --docker-password $_password --docker-email not@used.com
    }
    else {
        kubectl delete secret acr-auth --namespace $namespace
        kubectl create secret docker-registry acr-auth --docker-server $acrLogin --docker-username $_clientId --docker-password $_password --docker-email not@used.com --namespace $namespace
    }
}

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Deploying secret for accessing ACR" -ForegroundColor Yellow
Write-Host "Additional parameters are: " -ForegroundColor Yellow
Write-Host "ACR $acrName in RG $resourceGroup " -ForegroundColor Yellow
Write-Host "Client Id: $clientId with pwd: $password " -ForegroundColor Yellow
Write-Host "-------------------------------------------------------- " -ForegroundColor Yellow

$acr=$(az acr show -n $acrName -g $resourceGroup -o json | ConvertFrom-Json)
$acrLogin=$acr.loginServer
$acrId=$acr.id
validate

if ([string]::IsNullOrEmpty($namespace)) {
    $namespace=$(kubectl config view --minify --output "jsonpath={..namespace}")
}

$acrCredentials = $(az acr credential show -n $acrName -g $resourceGroup -o json | ConvertFrom-Json)
if ($acrCredentials) {
    Write-Host "Creating ACR auth based secret..." -ForegroundColor Yellow
    $acrPwd=$acrCredentials.passwords[0].value
    $acrUser=$acrCredentials.username
    createSecret $acrUser $acrPwd
}
elseif (-not [string]::IsNullOrEmpty($clientId)) {
    Write-Host "Creating Service Principal based auth..." -ForegroundColor Yellow
    az role assignment create --assignee $clientId --scope $acrId --role reader
    createSecret $clientId $password
}
else {
    Write-Host "Couldn't create Service Principal to access ACR." -ForegroundColor Red
    exit 1
}

Write-Host "Deploying ServiceAccount ttsa" -ForegroundColor Yellow

Push-Location $($MyInvocation.InvocationName | Split-Path)
$ttsaPath=$(./Join-Path-Recursively.ps1 -pathParts ..,helm,ttsa.yaml)
if ([string]::IsNullOrEmpty($namespace)) {
    kubectl apply -f $ttsaPath
}
else {
    kubectl apply -f $ttsaPath -n $namespace
}
Pop-Location
#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$false)][string]$clientId,
    [parameter(Mandatory=$false)][string]$password,
    [parameter(Mandatory=$false)][bool]$deployWinLinux,
    [parameter(Mandatory=$false)][bool]$deployAks=$true
)
$spCreated=$false
$sourceFolder=$(Join-Path -Path .. -ChildPath arm)

Push-Location $($MyInvocation.InvocationName | Split-Path)

$script="deployment.json"

if($deployWinLinux) {
    $script="deployment-dual-nodes.json"
}

if (-not $deployAks) {
    $script="deployment-no-aks.json"
}

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Deploying ARM script $script" -ForegroundColor Yellow
Write-Host "-------------------------------------------------------- " -ForegroundColor Yellow

$rg = $(az group show -n $resourceGroup -o json | ConvertFrom-Json)
# Deployment without AKS can be done in a existing or non-existing resource group.
if (-not $rg) {
    Write-Host "Creating resource group $resourceGroup in $location" -ForegroundColor Yellow
    az group create -n $resourceGroup -l $location
}

if ($deployAks) {
    if (-not $clientId -or -not $password) {
        Write-Host "Service principal will be created..." -ForegroundColor Yellow
        $sp = $(az ad sp create-for-rbac -o json | ConvertFrom-Json)
        $clientId = $sp.appId
        $password = $sp.password
        $spCreated=$true
    }

    Write-Host "Getting last AKS version in location $location" -ForegroundColor Yellow
    $aksVersions=$(az aks get-versions -l $location --query  orchestrators[].orchestratorVersion -o json | ConvertFrom-Json)
    $aksLastVersion=$aksVersions[$aksVersions.Length-1]
    Write-Host "AKS last version is $aksLastVersion" -ForegroundColor Yellow

    Write-Host "Begining the ARM deployment..." -ForegroundColor Yellow
    Push-Location $sourceFolder
    az group deployment create -g $resourceGroup --template-file $script --parameters servicePrincipalId=$clientId --parameters servicePrincipalSecret=$password --parameters aksVersion=$aksLastVersion
    Pop-Location 
}
else {
    Write-Host "Begining the ARM deployment..." -ForegroundColor Yellow
    Push-Location $sourceFolder
    az group deployment create -g $resourceGroup --template-file $script
    Pop-Location 
}

if ($spCreated) {
    Write-Host "-----------------------------------------" -ForegroundColor Yellow
    Write-Host "Details of the Service Principal Created:" -ForegroundColor Yellow
    Write-Host ($sp | ConvertTo-Json) -ForegroundColor Yellow
    Write-Host "-----------------------------------------" -ForegroundColor Yellow
}

Pop-Location 

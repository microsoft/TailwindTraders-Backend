Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$false)][string]$clientId,
    [parameter(Mandatory=$false)][string]$password,
    [parameter(Mandatory=$false)][string]$dbAdmin="ttadmin",
    [parameter(Mandatory=$false)][string]$dbPassword="Passw0rd1!"
)

$script="./deployment.json"
$spCreated=$false

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Deploying ARM script $deployment" -ForegroundColor Yellow
Write-Host "-------------------------------------------------------- " -ForegroundColor Yellow

$rg = $(az group show -n $resourceGroup -o json | ConvertFrom-Json)
if ($rg) {
    Write-Host "Resource group $resourceGroup already exists. Exiting." -ForegroundColor Red
    exit 1
}

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
az group create -n $resourceGroup -l $location
az group deployment create -g $resourceGroup --template-file deployment.json --parameters servicePrincipalId=$clientId --parameters servicePrincipalSecret=$password --parameters sqlServerAdministratorLogin=$dbAdmin --parameters sqlServerAdministratorLoginPassword=$dbPassword --parameters aksVersion=$aksLastVersion

Write-Host "Creating stockdb database in PostgreSQL" -ForegroundColor Yellow
$pgs = $(az postgres server list -g $resourceGroup -o json | ConvertFrom-Json)
$pg=$pgs[0]
Write-Host "PostgreSQL server is $($pg.name)" -ForegroundColor Yellow
az postgres db create -g $resourceGroup -s $pg.name -n stockdb

if ($spCreated) {
    Write-Host "-----------------------------------------" -ForegroundColor Yellow
    Write-Host "Details of the Service Principal Created:" -ForegroundColor Yellow
    Write-Host ($sp | ConvertTo-Json) -ForegroundColor Yellow
    Write-Host "-----------------------------------------" -ForegroundColor Yellow
}

Write-Host "-----------------------------------------" -ForegroundColor Yellow
Write-Host "Db admin: $dbAdmin"
Write-Host "Db Admin Pwd: $dbPassword"
Write-Host "-----------------------------------------" -ForegroundColor Yellow



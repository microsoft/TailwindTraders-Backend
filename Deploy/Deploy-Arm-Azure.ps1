Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$false)][string]$clientId,
    [parameter(Mandatory=$false)][string]$password,
    [parameter(Mandatory=$false)][string]$dbAdmin="ttadmin",
    [parameter(Mandatory=$false)][string]$dbPassword="Passw0rd1!",
    [parameter(Mandatory=$false)][bool]$deployAks=$true
)
$spCreated=$false
$script="./deployment.json"

if (-not $deployAks) {
    $script="./deployment-no-aks.json"
}

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Deploying ARM script $script" -ForegroundColor Yellow
Write-Host "-------------------------------------------------------- " -ForegroundColor Yellow

$rg = $(az group show -n $resourceGroup -o json | ConvertFrom-Json)
if ($deployAks) {
    # Deployment including AKS must be done in a non-existent resource group
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
    az group deployment create -g $resourceGroup --template-file $script --parameters servicePrincipalId=$clientId --parameters servicePrincipalSecret=$password --parameters sqlServerAdministratorLogin=$dbAdmin --parameters sqlServerAdministratorLoginPassword=$dbPassword --parameters aksVersion=$aksLastVersion
}
else {
    # Deployment without AKS can be done in a existing or non-existing resource group.
    if (-not $rg) {
        Write-Host "Creating resource group $resourceGroup in $location"
        az group create -n $resourceGroup -l $location
    }
    Write-Host "Begining the ARM deployment..." -ForegroundColor Yellow
    az group deployment create -g $resourceGroup --template-file $script --parameters sqlServerAdministratorLogin=$dbAdmin --parameters sqlServerAdministratorLoginPassword=$dbPassword
}

Write-Host "Creating stockdb database in PostgreSQL" -ForegroundColor Yellow
$pgs = $(az postgres server list -g $resourceGroup -o json | ConvertFrom-Json)
$pg=$pgs[0]
Write-Host "PostgreSQL server is $($pg.name)" -ForegroundColor Yellow
az postgres db create -g $resourceGroup -s $pg.name -n stockdb
Write-Host "Creating Firewall rule in $($pg.name) to allow connection from Azure Services" -ForegroundColor Yellow
az postgres server firewall-rule create --end-ip-address 0.0.0.0 --start-ip-address 0.0.0.0 --name AllowAllWindowsAzureIps -g $resourceGroup --server-name $pg.name
Write-Host "Disabling ssl enforcement for $($pg.name)" -ForegroundColor Yellow
az postgres server update -g $resourceGroup --ssl-enforcement Disabled --name $pg.name

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



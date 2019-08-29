Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$true)][string]$clientId,
    [parameter(Mandatory=$true)][string]$password,
    [parameter(Mandatory=$true)][string]$winUser,
    [parameter(Mandatory=$true)][string]$winPassword,
    [parameter(Mandatory=$false)][string]$aksName,
    [parameter(Mandatory=$false)][int]$linuxNodes=3,
    [parameter(Mandatory=$false)][int]$winNodes=2
)

$rg = $(az group show -n $resourceGroup -o json | ConvertFrom-Json)

if (-not $rg) {
    $rg=$(az group create -n $resourceGroup -l $location)
}

Write-Host "Install required az-cli extension aks-preview" -ForegroundColor Yellow
az extension add --name aks-preview

Write-Host "Getting last AKS version in location $location" -ForegroundColor Yellow
$aksVersions=$(az aks get-versions -l $location --query  orchestrators[].orchestratorVersion -o json | ConvertFrom-Json)
$aksLastVersion=$aksVersions[$aksVersions.Length-1]

Write-Host "AKS last version is $aksLastVersion" -ForegroundColor Yellow
if (-not $aksLastVersion.StartsWith("1.14")) {
    Write-Host "AKS 1.14 required. Exiting." -ForegroundColor Red
    exit 1
}

if ([String]::IsNullOrEmpty($aksName)) {
    $aksName=$resourceGroup
}

Write-Host "Creating AKS..." -ForegroundColor Yellow
az aks create --resource-group $resourceGroup `
    --name $aksName `
    --node-count $linuxNodes `
    --enable-addons monitoring `
    --generate-ssh-keys `
    --kubernetes-version $aksLastVersion   `
    --windows-admin-username $winUser  `
    --windows-admin-password $winPassword  `
    --enable-vmss `
    --network-plugin azure  `
    --service-principal $clientId `
    --client-secret $password

Write-Host "Configuring AKS nodes..." -ForegroundColor Yellow
az aks nodepool add --resource-group $resourceGroup `
    --cluster-name $aksName `
    --os-type Windows `
    --name npwin `
    --node-count $winNodes `
    --kubernetes-version $aksLastVersion
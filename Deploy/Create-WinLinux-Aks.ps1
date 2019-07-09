Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$true)][string]$clientId,
    [parameter(Mandatory=$true)][string]$password,
    [parameter(Mandatory=$true)][string]$winUser,
    [parameter(Mandatory=$true)][string]$winPassword
)

$rg = $(az group show -n $resourceGroup -o json | ConvertFrom-Json)

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

Write-Host "Creating AKS..." -ForegroundColor Yellow
az aks create --resource-group $resourceGroup `
    --name $resourceGroup `
    --node-count 2 `
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
    --cluster-name $resourceGroup `
    --os-type Windows `
    --name npwin `
    --node-count 2 `
    --kubernetes-version $aksLastVersion
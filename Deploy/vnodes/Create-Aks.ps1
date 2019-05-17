Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$location="eastus2",
    [parameter(Mandatory=$false)][string]$aksName="ttvnodes"
)

$rg = $(az group show -g $resourceGroup)

if (-not $rg) {
    Write-Host "Resource group $resourceGroup not found. Creating in $location" -ForegroundColor Yellow
    az group create -n $resourceGroup -l $location
}

$vnetName="ttaksvnet"
$vsubnetName="ttakssubnet"
$vnodesSubnetName="ttaksvnodessubnet"

Write-Host "Creating a vnet" -ForegroundColor Yellow
az network vnet create --resource-group $resourceGroup --name $vnetName  --address-prefixes 10.0.0.0/8 --subnet-name $vsubnetName --subnet-prefix 10.240.0.0/16
Write-Host "Creating a subnet" -ForegroundColor Yellow
az network vnet subnet create --resource-group $resourceGroup --vnet-name $vnetName --name $vnodesSubnetName --address-prefixes 10.241.0.0/16

Write-Host "Creating a service principal..." -ForegroundColor Yellow
$sp=$(az ad sp create-for-rbac --skip-assignment -o json | ConvertFrom-Json)
$clientid=$sp.appId
$password=$sp.password
Write-Host "Principal appid is $clientid and pwd is $password" -ForegroundColor Yellow

Write-Host "Waiting 1min to ensure principal is ready..."
Start-Sleep -Seconds 60

Write-Host "Adding permissions to vnet..." -ForegroundColor Yellow
$vnetid = $(az network vnet show --resource-group $resourceGroup --name $vnetName --query id -o tsv)
Write-Host "vnet ID is $vnetid"
cmd /c "az role assignment create --assignee $clientid --scope $vnetid --role Contributor"
Write-Host "Creating the AKS in the subnet..." -ForegroundColor Yellow
$subnetId=$(az network vnet subnet show --resource-group $resourceGroup --vnet-name $vnetName --name $vsubnetName --query id -o tsv)
Write-Host "subnet ID is $subnetId"
az aks create --resource-group $resourceGroup --name $aksName --node-count 3 --network-plugin azure --service-cidr 10.0.0.0/16 --dns-service-ip 10.0.0.10 --docker-bridge-address 172.17.0.1/16 --vnet-subnet-id $subnetId --service-principal $clientid  --client-secret $password
Write-Host "Enabling vnodes in the AKS..." -ForegroundColor Yellow
az aks enable-addons --resource-group $resourceGroup --name $aksName --addons virtual-node  --subnet-name $vnodesSubnetName

Write-Host "Enabling http application rotuing"
az aks enable-addons --addons http_application_routing -n $aksName -g $resourceGroup

Write-Host "Adding credentials to kubectl..."
az aks get-credentials -n $aksName  -g $resourceGroup

Write-Host "Showing nodes. Node named 'virtual-node-aci-linux' should appear"
kubectl get nodes

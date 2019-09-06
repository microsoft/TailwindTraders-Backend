Param (
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$dockerTag = "latest",
    [parameter(Mandatory=$false)][string]$name = "tailwindtraders",
    [parameter(Mandatory=$false)][bool]$buildDocker = $true,
    [parameter(Mandatory=$false)][string]$namespace = "",
    [parameter(Mandatory=$false)][string][ValidateSet('prod','staging','none', IgnoreCase=$false)]$sslSupport = "staging",
    [parameter(Mandatory=$false)][string]$charts = "infra,*"
)

#############################################################################
# Check for RG
#############################################################################

$rg=$(az group show -n $resourceGroup -o json | ConvertFrom-Json)

if (-not $rg) {
    Write-Host "Fatal: Resource group not found" -ForegroundColor Red
    exit 1
}

#############################################################################
# Check for AKS and getting AKS credentials
#############################################################################

$all_aks=$(az aks list -g $resourceGroup -o json --query "[].{name: name, host: addonProfiles.httpapplicationrouting.config.HTTPApplicationRoutingZoneName}" | ConvertFrom-Json)

if ($all_aks.Length -eq 0) {
    Write-Host "Fatal: No AKS found in RG $resourceGroup" -ForegroundColor Red
    exit 1
}
if  ($all_aks.Length -gt 1) {
    Write-Host "Fatal: More than one AKS found in RG $resourceGroup" -ForegroundColor Red
    exit 1
}

$aks=$all_aks[0]

Write-Host "Found AKS $($aks.name) in RG $resourceGroup." -ForegroundColor Yellow
az aks get-credentials -n $aks.name -g $resourceGroup

#############################################################################
# Check for Tiller
#############################################################################

$tillerDeploy = $(kubectl get deployments -n kube-system tiller-deploy  --no-headers -o custom-columns=:metadata.name)

if ([String]::IsNullOrEmpty($tillerDeploy)) {
    Write-Host "Tiller is not running on the cluster. Installing..." -ForegroundColor Yellow
    & .\Add-Tiller.ps1 
}
else {
    Write-Host "Tiller found up & running. Skipping Tiller installation..." -ForegroundColor Yellow
}

##############################################################################
# Check for ACR and installing secret in AKS
##############################################################################

$all_acrs=$(az acr list -g $resourceGroup -o json --query "[].{name: name, loginServer: loginServer, adminUserEnabled: adminUserEnabled}" | ConvertFrom-Json)
if ($all_acrs.Length -eq 0) {
    Write-Host "Fatal: No ACR found in RG $resourceGroup" -ForegroundColor Red 
    exit 1
}
if  ($all_acrs.Length -gt 1) {
    Write-Host "Fatal: More than one ACR found in RG $resourceGroup" -ForegroundColor Red
    exit 1
}

$acr=$all_acrs[0]

if (-not $acr.adminUserEnabled) {
    Write-Host "ACR $($acr.name) has not admin login enabled. Enabling it..." -ForegroundColor Yellow
    az acr update -n $acr.name --admin-enabled true
}

Write-Host "Found ACR $($acr.name) in RG $resourceGroup." -ForegroundColor Yellow

###################################################################################
# Building and pushing images
###################################################################################
if ($buildDocker) {
    Write-Host "Building and Pushing Docker Images to $($acr.loginServer)" -ForegroundColor Yellow
    & .\Build-Push.ps1 -resourceGroup $resourceGroup -acrName $acr.name -dockerTag $dockerTag
}

###################################################################################
# Installing secret on the cluster
###################################################################################
Write-Host "Installing secret on AKS $($aks.name)"
$acrCredentials=$(az acr credential show -g $resourceGroup -n $acr.name -o json | ConvertFrom-Json)
& "kubectl" delete secret acr-auth
& "kubectl" create secret docker-registry acr-auth --docker-server $($acr.loginServer) --docker-username $($acrCredentials.username) --docker-password $($acrCredentials.passwords[0].value) --docker-email not@used.com
Write-Host "Deploying ServiceAccount ttsa" -ForegroundColor Yellow
kubectl apply -f helm/ttsa.yaml

##################################################################################
# Delete older releases
##################################################################################
Write-Host "Releases named $name found. Removing..." -ForegroundColor Yellow
$oldReleases=$(helm ls $name -q)
if ($oldReleases) {
    helm delete $oldReleases --purge 
}

###################################################################################
# Deploy TLS staging environment
###################################################################################
Write-Host "Deploying cert manager on AKS $($aks.name)" -ForegroundColor Yellow
& .\Add-Cert-Manager.ps1
& .\Enable-SSl.ps1 -sslSupport $sslSupport -aksName $aks.name -resourceGroup $resourceGroup -name $name

###################################################################################
# Deploy Tailwind Traders images on cluster
###################################################################################ç
Write-Host "Deploying Tailwind Traders images on AKS $($aks.name)" -ForegroundColor Yellow
& .\Deploy-CosmosDb-Aci.ps1 -resourceGroup $resourceGroup -name "$name-cart"
& .\Deploy-Images-Aks.ps1 -aksName $aks.name -resourceGroup $resourceGroup -cartAciGroup $resourceGroup -cartAciName "$name-cart" -acrName $acr.name -useInfraInAks $true -charts $charts -name $name -namespace $namespace
& .\Deploy-Pictures-Aks.ps1 -resourceGroup $resourceGroup -aksName $aks.name
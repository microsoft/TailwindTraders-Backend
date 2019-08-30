Param(
    [parameter(Mandatory=$false)][string]$name = "tailwindtraders",
    [parameter(Mandatory=$false)][string]$aksName,
    [parameter(Mandatory=$false)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$acrName,
    [parameter(Mandatory=$false)][string]$tag="latest",
    [parameter(Mandatory=$false)][string]$charts = "*",
    [parameter(Mandatory=$false)][string]$valuesFile = "",
    [parameter(Mandatory=$false)][bool]$useInfraInAks=$false,
    [parameter(Mandatory=$false)][string]$cartAciGroup="",
    [parameter(Mandatory=$false)][string]$cartAciName="",
    [parameter(Mandatory=$false)][string]$afHost = "http://your-product-visits-af-here",
    [parameter(Mandatory=$false)][string]$namespace = "",
    [parameter(Mandatory=$false)][string][ValidateSet('prod','staging','none','custom', IgnoreCase=$false)]$tlsEnv = "none",
    [parameter(Mandatory=$false)][string]$tlsHost="",
    [parameter(Mandatory=$false)][string]$tlsSecretName="tt-tls-custom",
    [parameter(Mandatory=$false)][bool]$autoscale=$false
)

function validate {
    $valid = $true

    if ([string]::IsNullOrEmpty($aksName)) {
        Write-Host "No AKS name. Use -aksName to specify name" -ForegroundColor Red
        $valid=$false
    }
    if ([string]::IsNullOrEmpty($resourceGroup))  {
        Write-Host "No resource group. Use -resourceGroup to specify resource group." -ForegroundColor Red
        $valid=$false
    }

    if ([string]::IsNullOrEmpty($aksHost) -and $tlsEnv -ne "custom")  {
        Write-Host "AKS host of HttpRouting can't be found. Are you using right AKS ($aksName) and RG ($resourceGroup)?" -ForegroundColor Red
        $valid=$false
    }     
    if ([string]::IsNullOrEmpty($acrLogin))  {
        Write-Host "ACR login server can't be found. Are you using right ACR ($acrName) and RG ($resourceGroup)?" -ForegroundColor Red
        $valid=$false
    }

    if ($useInfraInAks -and [string]::IsNullOrEmpty($cartAciName)) {
        Write-Host "If using infrastructure in ACR must use -cartAciName and -cartAciGroup to set the ACI container running the cosmosdb emulator"
        $valid=$false
    }

    if ($tlsEnv -eq "custom" -and [string]::IsNullOrEmpty($tlsSecretName)) {
        Write-Host "If tlsEnv is custom must use -tlsSecretName to set the TLS secret name (you need to install this secret manually)"
        $valid=$false
    }

    if ($tlsEnv -eq "custom" -and [string]::IsNullOrEmpty($tlsHost)) {
        Write-Host "If tlsEnv is custom must use -tlsHost to set the hostname of AKS (inferred name of Http Application Routing won't be used)"
        $valid=$false
    }

    if ($valid -eq $false) {
        exit 1
    }
}

function createHelmCommand([string]$command) {
    $tlsSecretNameToUse = ""
    if ($tlsEnv -eq "staging") {
        $tlsSecretNameToUse = "tt-letsencrypt-staging"
    }
    if ($tlsEnv -eq "prod") {
        $tlsSecretNameToUse = "tt-letsencrypt-prod"
    }
    if ($tlsEnv -eq "custom") {
        $tlsSecretNameToUse=$tlsSecretName
    }	    

    $newcmd = $command

    if (-not [string]::IsNullOrEmpty($namespace)) {
        $newcmd = "$newcmd --namespace $namespace" 
    }

    if (-not [string]::IsNullOrEmpty($tlsSecretNameToUse)) {
        $newcmd = "$newcmd --set ingress.tls[0].secretName=$tlsSecretNameToUse --set ingress.tls[0].hosts={$aksHost}"
    }

    return "$newcmd";
}

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host " Deploying images on cluster $aksName"  -ForegroundColor Yellow
Write-Host " "  -ForegroundColor Yellow
Write-Host " Additional parameters are:"  -ForegroundColor Yellow
Write-Host " Release Name: $name"  -ForegroundColor Yellow
Write-Host " AKS to use: $aksName in RG $resourceGroup and ACR $acrName"  -ForegroundColor Yellow
Write-Host " Images tag: $tag"  -ForegroundColor Yellow
Write-Host " TLS/SSL environment to enable: $tlsEnv"  -ForegroundColor Yellow
Write-Host " Namespace (empty means the one in .kube/config): $namespace"  -ForegroundColor Yellow
Write-Host " --------------------------------------------------------" 

$acrLogin=$(az acr show -n $acrName -g $resourceGroup -o json| ConvertFrom-Json).loginServer

if ($tlsEnv -ne "custom") {
    $aksHost=$(az aks show -n $aksName -g $resourceGroup --query addonProfiles.httpapplicationrouting.config.HTTPApplicationRoutingZoneName -o json | ConvertFrom-Json)

     if (-not $aksHost) {
        $aksHost=$(az aks show -n $aksName -g $resourceGroup --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName -o json | ConvertFrom-Json)
    }

     Write-Host "acr login server is $acrLogin" -ForegroundColor Yellow
    Write-Host "aksHost is $aksHost" -ForegroundColor Yellow
}
else {
    $aksHost=$tlsHost
}

validate

Push-Location helm

Write-Host "Deploying charts $charts" -ForegroundColor Yellow

$cartAci=$null
$azuriteUrl=$null
$azuriteProductsUrl=$null
$azuriteProfilesUrl=$null
$azuriteProductsDetailsUrl=$null
$azuriteCouponsUrl=$null

if ($useInfraInAks) {
    Write-Host "charts $charts will be configured to use internal AKS infrastructure." -ForegroundColor Yellow  
    if ([String]::IsNullOrEmpty($valuesFile)) {
        $valuesFile="gvalues_inf.yaml"
    }
    $azuriteUrl="http://$aksHost/blobs/devstoreaccount1"
    $azuriteProductsUrl="$azuriteUrl/product-list"
    $azuriteProfilesUrl="$azuriteUrl/profiles-list"
    $azuriteProductsDetailsUrl="$azuriteUrl/product-detail"
    $azuriteCouponsUrl="$azuriteUrl/coupon-list"
    Write-Host "Getting info of ACI $cartAciGroup/$cartAciName"
    Write-Host "az container show -g $cartAciGroup -n $cartAciName"
    $cartAci=$(az container show -g $cartAciGroup -n $cartAciName -o json | ConvertFrom-Json)
    Write-Host "ACI Cart running CosmosDb emulator is on " $cartAci.ipAddress.fqdn -ForegroundColor Yellow
    if ([String]::IsNullOrEmpty($cartAci.ipAddress.fqdn)) {
        Write-Host "ACI Cart not found or it has no fqdn. Please run Deploy-CosmosDb.ps1" -ForegroundColor Red
        exit 1
    }
}
else {
    if ([String]::IsNullOrEmpty($valuesFile)) {
        $valuesFile="gvalues.yaml"
    }
}

Write-Host "Configuration file used is $valuesFile" -ForegroundColor Yellow

if ($charts.Contains("pr") -or  $charts.Contains("*")) {
    Write-Host "Products chart - pr" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-product products-api -f $valuesFile --set az.productvisitsurl=$afHost --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/product.api --set image.tag=$tag --set hpa.activated=$autoscale"
    if ($useInfraInAks) {
        $command = "$command --set inf.storage.productimages=$azuriteProductsUrl --set inf.storage.productdetailimages=$azuriteProductsDetailsUrl"
    }
    $command = createHelmCommand $command 
    cmd /c "$command"
}

if ($charts.Contains("cp") -or  $charts.Contains("*")) {
    Write-Host "Coupons chart - cp" -ForegroundColor Yellow
    $command="helm upgrade --install $name-coupon coupons-api -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/coupon.api --set image.tag=$tag  --set hpa.activated=$autoscale"
    if ($useInfraInAks) {
        $command = "$command --set inf.storage.couponimage=$azuriteCouponsUrl"
    }
    $command = createHelmCommand $command
    cmd /c "$command"
}

if ($charts.Contains("pf") -or  $charts.Contains("*")) {
    Write-Host "Profile chart - pf " -ForegroundColor Yellow
    $command = "helm upgrade --install $name-profile profiles-api -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/profile.api --set image.tag=$tag --set hpa.activated=$autoscale"
    if ($useInfraInAks) {
        $command = "$command --set inf.storage.profileimages=$azuriteProfilesUrl"
    }
    $command = createHelmCommand $command 
    cmd /c "$command"
}

if ($charts.Contains("pp") -or  $charts.Contains("*")) {
    Write-Host "Popular products chart - pp" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-popular-product popular-products-api -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/popular-product.api --set image.tag=$tag --set initImage.repository=$acrLogin/popular-product-seed.api  --set initImage.tag=$tag --set hpa.activated=$autoscale"
    if ($useInfraInAks) {
        $command = "$command --set inf.storage.productimages=$azuriteProductsUrl"
    }
    $command = createHelmCommand $command
    cmd /c "$command"
}

if ($charts.Contains("st") -or  $charts.Contains("*")) {
    Write-Host "Stock -st" -ForegroundColor Yellow
    $command = createHelmCommand "helm  upgrade --install $name-stock stock-api -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/stock.api --set image.tag=$tag --set hpa.activated=$autoscale"
    cmd /c "$command"
}

if ($charts.Contains("ic") -or  $charts.Contains("*")) {
    Write-Host "Image Classifier -ic" -ForegroundColor Yellow
    $command = createHelmCommand "helm  upgrade --install $name-image-classifier image-classifier-api -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/image-classifier.api --set image.tag=$tag --set hpa.activated=$autoscale"
    cmd /c "$command"
}

if ($charts.Contains("ct") -or  $charts.Contains("*")) {
    Write-Host "Cart (Basket) -ct" -ForegroundColor Yellow
    $command = "helm  upgrade --install $name-cart cart-api -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/cart.api --set image.tag=$tag --set hpa.activated=$autoscale"
    if ($useInfraInAks) {
        $fqdn=$cartAci.ipAddress.fqdn
        $command = "$command --set inf.db.cart.host=https://${fqdn}:8081 -f values_inf_cartapi.yaml"
    }
    $command = createHelmCommand $command
    cmd /c "$command"
}

if ($charts.Contains("lg") -or  $charts.Contains("*")) {
    Write-Host "Login -lg" -ForegroundColor Yellow
    $command = "helm  upgrade --install $name-login login-api -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/login.api --set image.tag=$tag --set hpa.activated=$autoscale"
    if ($useInfraInAks) {
        $command = "$command --set inf.storage.profileimages=$azuriteProfilesUrl"
    }
    $command = createHelmCommand $command
    cmd /c "$command"
}

if ($charts.Contains("rr") -or  $charts.Contains("*")) {
    Write-Host "Rewards Registration -rr" -ForegroundColor Yellow
    $command = createHelmCommand "helm  upgrade --install $name-rewards-registration rewards-registration-api -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/rewards.registration.api --set image.tag=$tag --set hpa.activated=$autoscale"
    cmd /c "$command"
}

if ($charts.Contains("mgw") -or  $charts.Contains("*")) {
    Write-Host "mobilebff -mgw" -ForegroundColor Yellow 
    $command = createHelmCommand "helm  upgrade --install $name-mobilebff mobilebff -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/mobileapigw --set image.tag=$tag --set hpa.activated=$autoscale"
    cmd /c "$command"
}

if ($charts.Contains("wgw") -or  $charts.Contains("*")) {
    Write-Host "webbff -wgw" -ForegroundColor Yellow
    $command = createHelmCommand "helm  upgrade --install $name-webbff webbff -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/webapigw --set image.tag=$tag --set hpa.activated=$autoscale"
    cmd /c "$command"
}

if ($charts.Contains("infra")) {
    Write-Host "*** Infrastructure ***" -ForegroundColor Green
    $command = createHelmCommand "helm  upgrade --install $name-infra infrastructure -f $valuesFile --set ingress.hosts={$aksHost}"
    cmd /c "$command"
}

Pop-Location

Write-Host "Tailwind traders deployed on AKS" -ForegroundColor Yellow
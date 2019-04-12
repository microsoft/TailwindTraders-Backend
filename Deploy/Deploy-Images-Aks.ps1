Param(
    [parameter(Mandatory=$false)][string]$name = "my-tt",
    [parameter(Mandatory=$false)][string]$aksName,
    [parameter(Mandatory=$false)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$acrName,
    [parameter(Mandatory=$false)][string]$tag="latest",
    [parameter(Mandatory=$false)][string]$charts = "*",
    [parameter(Mandatory=$false)][string]$valuesFile = "gvalues.yaml",
    [parameter(Mandatory=$false)][bool]$useInfraInAks=$false,
    [parameter(Mandatory=$false)][string]$cartAciGroup="",
    [parameter(Mandatory=$false)][string]$cartAciName="",
    [parameter(Mandatory=$false)][string]$afHost = "http://your-product-visits-af-here",
    [parameter(Mandatory=$false)][string][ValidateSet('prod','staging','none', IgnoreCase=$false)]$tlsEnv = "none"
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

    if ([string]::IsNullOrEmpty($aksHost))  {
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

    if ($valid -eq $false) {
        exit 1
    }
}

function createHelmCommand([string]$command, $chart) {
    $tlsSecretName = ""
    if ($tlsEnv -eq "staging") {
        $tlsSecretName = "tt-letsencrypt-staging"
    }
    if ($tlsEnv -eq "prod") {
        $tlsSecretName = "tt-letsencrypt-prod"
    }

    $newcmd = $command

    if (-not [string]::IsNullOrEmpty($tlsSecretName)) {
        $newcmd = "$newcmd --set ingress.tls[0].secretName=$tlsSecretName --set ingress.tls[0].hosts={$aksHost}"
    }

    $newcmd = "$newcmd $chart"
    return $newcmd;
}


Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host " Deploying images on cluster $aksName"  -ForegroundColor Yellow
Write-Host " "  -ForegroundColor Yellow
Write-Host " Additional parameters are:"  -ForegroundColor Yellow
Write-Host " Release Name: $name"  -ForegroundColor Yellow
Write-Host " AKS to use: $aksName in RG $resourceGroup and ACR $acrName"  -ForegroundColor Yellow
Write-Host " Images tag: $tag"  -ForegroundColor Red
Write-Host " TLS/SSL environment to enable: $tlsEnv"  -ForegroundColor Red
Write-Host " --------------------------------------------------------" 

$acrLogin=$(az acr show -n $acrName -g $resourceGroup | ConvertFrom-Json).loginServer
$aksHost=$(az aks show -n $aksName -g $resourceGroup | ConvertFrom-Json).addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName

Write-Host "acr login server is $acrLogin" -ForegroundColor Yellow
Write-Host "aksHost is $aksHost" -ForegroundColor Yellow

validate

Push-Location helm

Write-Host "Deploying charts $charts" -ForegroundColor Yellow

$cartAci=$null

if ($useInfraInAks) {
    Write-Host "charts $charts will be configured to use internal AKS infrastructure. Value of --valuesFile is ingored" -ForegroundColor Yellow  
    $valuesFile="gvalues_inf.yaml"
    Write-Host "Getting info of ACI $cartAciGroup/$cartAciName"
    Write-Host "az container show -g $cartAciGroup -n $cartAciName"
    $cartAci=$(az container show -g $cartAciGroup -n $cartAciName | ConvertFrom-Json)
    Write-Host "ACI Cart running CosmosDb emulator is on " $cartAci.ipAddress.fqdn -ForegroundColor Yellow
    if ([String]::IsNullOrEmpty($cartAci.ipAddress.fqdn)) {
        Write-Host "ACI Cart not found or it has no fqdn. Please run Deploy-CosmosDb.ps1"
    }

}



if ($charts.Contains("pr") -or  $charts.Contains("*")) {
    Write-Host "Products chart - pr" -ForegroundColor Yellow
    $command = createHelmCommand "helm install --name $name-product -f $valuesFile --set az.productvisitsurl=$afHost --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/product.api --set image.tag=$tag"  "products-api" 
    cmd /c "$command"
}

if ($charts.Contains("cp") -or  $charts.Contains("*")) {
    Write-Host "Coupons chart - cp" -ForegroundColor Yellow
    $command = createHelmCommand  "helm install --name $name-coupon -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/coupon.api --set image.tag=$tag" "coupons-api"
    cmd /c "$command"
}

if ($charts.Contains("pf") -or  $charts.Contains("*")) {
    Write-Host "Profile chart - pf " -ForegroundColor Yellow
    $command = createHelmCommand "helm install --name $name-profile -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/profile.api --set image.tag=$tag" "profiles-api"
    cmd /c "$command"
}

if ($charts.Contains("pp") -or  $charts.Contains("*")) {
    Write-Host "Popular products chart - pp" -ForegroundColor Yellow
    $command = createHelmCommand "helm install --name $name-popular-product -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/popular-product.api --set image.tag=$tag --set initImage.repository=$acrLogin/popular-product-seed.api  --set initImage.tag=$tag" "popular-products-api"
    cmd /c "$command"
}

if ($charts.Contains("st") -or  $charts.Contains("*")) {
    Write-Host "Stock -st" -ForegroundColor Yellow
    $command = createHelmCommand "helm  install --name $name-stock -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/stock.api --set image.tag=$tag" "stock-api"
    cmd /c "$command"
}

if ($charts.Contains("ic") -or  $charts.Contains("*")) {
    Write-Host "Image Classifier -ic" -ForegroundColor Yellow
    $command = createHelmCommand "helm  install --name $name-image-classifier -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/image-classifier.api --set image.tag=$tag" "image-classifier-api"
    cmd /c "$command"
}

if ($charts.Contains("ct") -or  $charts.Contains("*")) {
    Write-Host "Cart (Basket) -ct" -ForegroundColor Yellow
    $command = "helm  install --name $name-cart -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/cart.api --set image.tag=$tag"
    if ($useInfraInAks) {
        $fqdn=$cartAci.ipAddress.fqdn
        $command = "$command --set inf.db.cart.host=https://${fqdn}:8081"
    }
    $command = createHelmCommand $command "cart-api"
    cmd /c "$command"
}

if ($charts.Contains("lg") -or  $charts.Contains("*")) {
    Write-Host "Login -lg" -ForegroundColor Yellow
    $command = createHelmCommand "helm  install --name $name-login -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/login.api --set image.tag=$tag" "login-api"
    cmd /c "$command"
}

if ($charts.Contains("mgw") -or  $charts.Contains("*")) {
    Write-Host "mobilebff -mgw" -ForegroundColor Yellow 
    $command = createHelmCommand "helm  install --name $name-mobilebff -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/mobileapigw --set image.tag=$tag" "mobilebff"
    cmd /c "$command"
}

if ($charts.Contains("wgw") -or  $charts.Contains("*")) {
    Write-Host "webbff -wgw" -ForegroundColor Yellow
    $command = createHelmCommand "helm  install --name $name-webbff -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/webapigw --set image.tag=$tag" "webbff"
    cmd /c "$command"
}

if ($charts.Contains("infra")) {
    Write-Host "*** Infrastructure ***" -ForegroundColor Green
    $command = createHelmCommand "helm  install --name $name-infra -f $valuesFile --set ingress.hosts={$aksHost}" "infrastructure"
    cmd /c "$command"
}

Pop-Location

Write-Host "Tailwind traders deployed on AKS" -ForegroundColor Yellow
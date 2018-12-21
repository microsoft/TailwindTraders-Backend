Param (
    [parameter(Mandatory=$false)][string][ValidateSet('prod','staging','none', IgnoreCase=$false)]$sslSupport = "none",
    [parameter(Mandatory=$false)][string]$name = "my-tt",
    [parameter(Mandatory=$false)][string]$aksName,
    [parameter(Mandatory=$false)][string]$resourceGroup 
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

    if ($sslSupport -eq "none")  {
        Write-Host "sslSupport set to none. Nothing will be done. Use staging or prod to setup SSL/TLS" -ForegroundColor Yellow
        $valid=$false
    }

    if ($valid -eq $false) {
        exit 1
    }
}

validate

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host " Enabling SSL/TLS support on cluster $aksName in RG $resourceGroup"  -ForegroundColor Yellow
Write-Host " --------------------------------------------------------" -ForegroundColor Yellow

if ([String]::IsNullOrEmpty($domain)) {
    $domain = $(az aks show -n $aksName -g $resourceGroup | ConvertFrom-Json).addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
}

if ([String]::IsNullOrEmpty($domain)) {
    Write-Host "Erorr: domain not passed and can't be inferred from AKS $aksName" -ForegroundColor Red
    exit 1
}

Write-Host "TLS/SSL will be bound to domain $domain"

Push-Location helm

if ($sslSupport -eq "staging") {
    Write-Host "Adding TLS/SSL support using Let's Encrypt Staging environment" -ForegroundColor Yellow
    Write-Host "helm install --name $name-ssl -f tls-support\values-staging.yaml --set domain=$domain tls-support" -ForegroundColor Yellow
    cmd /c "helm install --name $name-ssl-staging -f tls-support\values-staging.yaml --set domain=$domain tls-support"
}
if ($sslSupport -eq "prod") {
    Write-Host "Adding TLS/SSL support using Let's Encrypt PRODUCTION environment" -ForegroundColor Yellow
    cmd /c "helm install --name $name-ssl-prod -f tls-support\values-prod.yaml --set domain=$domain tls-support"
}

Pop-Location


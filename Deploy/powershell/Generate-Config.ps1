#! /usr/bin/pwsh

Param (
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string[]]$outputFile=$null,
    [parameter(Mandatory=$false)][string]$rewardsResourceGroup="",
    [parameter(Mandatory=$false)][string]$rewardsDbPassword="",
    [parameter(Mandatory=$false)][string[]]$gvaluesTemplate="..,helm,gvalues.template",
    [parameter(Mandatory=$false)][string]$ingressClass="addon-http-application-routing"
)

function EnsureAndReturnFirstItem($arr, $restype) {
    if (-not $arr -or $arr.Length -ne 1) {
        Write-Host "Fatal: No $restype found (or found more than one)" -ForegroundColor Red
        exit 1
    }

    return $arr[0]
}

# Check the rg
$rg=$(az group show -n $resourceGroup -o json | ConvertFrom-Json)

if (-not $rg) {
    Write-Host "Fatal: Resource group not found" -ForegroundColor Red
    exit 1
}

### Getting Resources
$tokens=@{}

## Getting storage info
$storage=$(az storage account list -g $resourceGroup --query "[].{name: name, blob: primaryEndpoints.blob}" -o json | ConvertFrom-Json)
$storage=EnsureAndReturnFirstItem $storage "Storage Account"
Write-Host "Storage Account: $($storage.name)" -ForegroundColor Yellow

## Getting CosmosDb info
$docdb=$(az cosmosdb list -g $resourceGroup --query "[?kind=='GlobalDocumentDB'].{name: name, kind:kind, documentEndpoint:documentEndpoint}" -o json | ConvertFrom-Json)
$docdb=EnsureAndReturnFirstItem $docdb "CosmosDB (Document Db)"
$docdbKey=$(az cosmosdb keys list -g $resourceGroup -n $docdb.name -o json --query primaryMasterKey | ConvertFrom-Json)
Write-Host "Document Db Account: $($docdb.name)" -ForegroundColor Yellow

$mongodb=$(az cosmosdb list -g $resourceGroup --query "[?kind=='MongoDB'].{name: name, kind:kind}" -o json | ConvertFrom-Json)
$mongodb=EnsureAndReturnFirstItem $mongodb "CosmosDB (MongoDb mode)"
$mongodbKey=$(az cosmosdb keys list -g $resourceGroup -n $mongodb.name -o json --query primaryMasterKey | ConvertFrom-Json)
Write-Host "Mongo Db Account: $($mongodb.name)" -ForegroundColor Yellow

If ($rewardsResourceGroup){
    $sqlsrv=$(az sql server list -g $rewardsResourceGroup --query "[].{administratorLogin:administratorLogin, name:name, fullyQualifiedDomainName: fullyQualifiedDomainName}" -o json | ConvertFrom-Json)
    $sqlsrv=EnsureAndReturnFirstItem $sqlsrv "SQL Server"
    Write-Host "Rewards Sql Server: $($sqlsrv.name)" -ForegroundColor Yellow

    if (-not $rewardsDbPassword) {
        Write-Host "Rewards registration requires the administrator password parameter --rewardsDbPassword" -ForegroundColor Red
        exit 1
    }

    $tokens.rewardshost=$sqlsrv.fullyQualifiedDomainName
    $tokens.rewardsuser=$sqlsrv.administratorLogin
    $tokens.rewardspwd=$rewardsDbPassword
}
else {
    $tokens.rewardshost="localdb"
    $tokens.rewardsuser="user"
    $tokens.rewardspwd="password"
}

## Getting App Insights instrumentation key, if required
$appinsightsId=""
$appInsightsName=$(az resource list -g $resourceGroup --resource-type Microsoft.Insights/components --query [].name | ConvertFrom-Json)
if ($appInsightsName -and $appInsightsName.Length -eq 1) {
    $appinsightsConfig=$(az monitor app-insights component show --app $appInsightsName[0] -g $resourceGroup -o json | ConvertFrom-Json)

    if ($appinsightsConfig) {
        $appinsightsId = $appinsightsConfig.instrumentationKey           
    }
}
Write-Host "App Insights Instrumentation Key: $appinsightsId" -ForegroundColor Yellow

## Showing Values that will be used

Write-Host "===========================================================" -ForegroundColor Yellow
Write-Host "gvalues file will be generated with values:"

$tokens.shoppinghost=$docdb.documentEndpoint
$tokens.shoppingauth=$docdbKey

$tokens.couponsuser=$mongodb.name
$tokens.couponshost="$($mongodb.name).documents.azure.com"
$tokens.couponspwd=$mongodbKey

$tokens.storage=$storage.blob
$tokens.rewardsregistration=If ($rewardsResourceGroup) { $true } Else { $false }
$tokens.appinsightsik=$appinsightsId

# Standard fixed tokens
$tokens.ingressclass=$ingressClass
$tokens.secissuer="TTFakeLogin"
$tokens.seckey="nEpLzQJGNSCNL5H6DIQCtTdNxf5VgAGcBbtXLms1YDD01KJBAs0WVawaEjn97uwB"
$tokens.ingressrewritepath=""
$tokens.ingressrewritetarget=""

if($ingressClass -eq "nginx") {
    $tokens.ingressrewritepath="(/|$)(.*)" 
    $tokens.ingressrewritetarget="`$2"
}

Write-Host ($tokens | ConvertTo-Json) -ForegroundColor Yellow
Write-Host "===========================================================" -ForegroundColor Yellow

Push-Location $($MyInvocation.InvocationName | Split-Path)
$gvaluesTemplatePath=$(./Join-Path-Recursively -pathParts $gvaluesTemplate.Split(","))
$outputFilePath=$(./Join-Path-Recursively -pathParts $outputFile.Split(","))
& ./Token-Replace.ps1 -inputFile $gvaluesTemplatePath -outputFile $outputFilePath -tokens $tokens
Pop-Location

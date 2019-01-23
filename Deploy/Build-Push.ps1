Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$acrName,
    [parameter(Mandatory=$false)][bool]$dockerBuild=$true,
    [parameter(Mandatory=$false)][bool]$dockerPush=$true,
    [parameter(Mandatory=$false)][string]$dockerTag="latest"
)

Write-Host "---------------------------------------------------" -ForegroundColor Yellow
Write-Host "Getting info from ACR $resourceGroup/$acrName" -ForegroundColor Yellow
Write-Host "---------------------------------------------------" -ForegroundColor Yellow
$acrLoginServer=$(az acr show -g $resourceGroup -n $acrName | ConvertFrom-Json).loginServer
$acrCredentials=$(az acr credential show -g $resourceGroup -n $acrName | ConvertFrom-Json)
$acrPwd=$acrCredentials.passwords[0].value
$acrUser=$acrCredentials.username

if ($dockerBuild) {
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Write-Host "Using docker compose to build & tag images." -ForegroundColor Yellow
    Write-Host "Images will be named as $acrLoginServer/imageName:$dockerTag" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow

    Push-Location ..\Source
    $env:TAG=$dockerTag
    $env:REGISTRY=$acrLoginServer 
    docker-compose build
    Pop-Location
}

if ($dockerPush) {
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Write-Host "Pushing images to $acrLoginServer" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Push-Location ..\Source
    docker login -p $acrPwd -u $acrUser $acrLoginServer
    $env:TAG=$dockerTag
    $env:REGISTRY=$acrLoginServer 
    docker-compose push
    Pop-Location
}




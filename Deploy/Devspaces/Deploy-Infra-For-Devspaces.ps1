# This script deploys ALL infrastructure in a AKS, and prepare it to use devspaces
Param (
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$dockerTag = "latest",
    [parameter(Mandatory=$false)][string]$name = "my-tt",
    [parameter(Mandatory=$false)][bool]$buildDocker = $true
)

Push-Location ..
& .\Full-Deploy-Infra-On-Aks.ps1 -resourceGroup $resourceGroup -dockerTag $dockerTag -name $name -buildDocker $buildDocker -charts infra -namespace ttinfra  -sslSupport none
Pop-Location


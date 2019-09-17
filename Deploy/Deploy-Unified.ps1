Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$false)][string]$clientId,
    [parameter(Mandatory=$false)][string]$password,
    [parameter(Mandatory=$false)][bool]$deployAks=$true
)

## Deploy-Arm-Azure.ps1
& ((Split-Path $MyInvocation.InvocationName) + "\Deploy-Arm-Azure.ps1") -resourceGroup $resourceGroup -location $location -clientId $clientId -password $password -deployAks $deployAks


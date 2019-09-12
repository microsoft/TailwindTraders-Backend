Param (
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$dockerTag = "latest",
    [parameter(Mandatory=$false)][string]$name = "tailwindtraders",
    [parameter(Mandatory=$false)][bool]$buildDocker = $true,
    [parameter(Mandatory=$false)][string]$namespace = "",
    [parameter(Mandatory=$false)][string][ValidateSet('prod','staging','none', IgnoreCase=$false)]$sslSupport = "staging",
    [parameter(Mandatory=$false)][string]$charts = "infra,*"
)


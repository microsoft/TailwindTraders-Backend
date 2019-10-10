#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$keyFile,
    [parameter(Mandatory=$true)][string]$cerFile,
    [parameter(Mandatory=$true)][string]$secretName
)

 Write-Host "Creating K8S TLS secret named $secretName from key $keyFile and cert $cerFile"

 kubectl create secret tls $secretName --key $keyFile --cert $cerFile
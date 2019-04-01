# This is to workaround issue #56 - https://github.com/Azure/dev-spaces/issues/56

Param(
    [parameter(Mandatory=$true)][string]$path,
    [parameter(Mandatory=$true)][string]$file
)


Write-Host "Copying $file to needed locations"
Copy-Item "$path\$file" -Destination ".\ApiGWs\Tailwind.Traders.WebBff\_gvalues.dev.yaml" -Force


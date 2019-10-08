Param(
    [parameter(Mandatory=$true)][string]$file
)


Write-Host "Copying $file to /Deploy/helm/gvalues.azds.yaml"
Copy-Item "$file" -Destination "..\..\helm\gvalues.azds.yaml" -Force


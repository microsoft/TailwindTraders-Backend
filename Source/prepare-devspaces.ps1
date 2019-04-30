# This is to workaround issue #56 - https://github.com/Azure/dev-spaces/issues/56

Param(
    [parameter(Mandatory=$true)][string]$file
)


Write-Host "Copying $file to /Deploy/helm/gvalues.azds.yaml"
Copy-Item "$file" -Destination "..\Deploy\helm\gvalues.azds.yaml" -Force


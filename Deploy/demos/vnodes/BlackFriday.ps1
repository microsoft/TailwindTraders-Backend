Param(
    [parameter(Mandatory=$false)][string]$aksHost,
    [parameter(Mandatory=$false)][string]$aksName,
    [parameter(Mandatory=$true)][string]$resourceGroup
)

workflow LoadTestUri {
    Param(
     [int]$parallelCount = 50,
     [int]$iterations = 100,
     [int]$minSleep = 50,
     [int]$maxSleep = 100,
     [string]$baseUrl
    )

    foreach -parallel ($x in 1..$parallelCount) {
      1..$iterations | %{ 
          $id=$(Get-Random -Max 100 -Minimum 1)
          if ($id -gt 50) {
              $url=$baseUrl
          }
          else {
            $url="$baseUrl/$id"
          }
          Write-Host ">> customer $x : iteration $_ -> $url" 
          $response = Invoke-WebRequest -Uri $url -UseBasicParsing
          $status = $response.StatusCode
          Write-Host "<< customer $x : iteration $_ : $status" -ForegroundColor Green
          $sleep=$(Get-Random -Max $maxSleep -Minimum $minSleep)
          [System.Threading.Thread]::Sleep($sleep)
      }
    }
}


if ([String]::IsNullOrEmpty($aksHost)) {
    $aksHost=$(az aks show -n $aksName -g $resourceGroup -o json --query addonProfiles.httpapplicationrouting.config.HTTPApplicationRoutingZoneName | ConvertFrom-Json)
    if (-not $aksHost) {
        $aksHost=$(az aks show -n $aksName -g $resourceGroup -o json --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName | ConvertFrom-Json)
    }

    if (-not $aksHost) {
        Write-Host "Could not infer URL for AKS $aksName in RG $resourceGroup"
        exit 1
    }
}

$url="http://$aksHost/product-api/v1/product"
Write-Host "Product API is in $url" -ForegroundColor Yellow

LoadTestUri -baseUrl $url

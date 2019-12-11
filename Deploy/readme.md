# Deploy folder

This folder contains a set of deployment scripts to cover various scenarios

## Powershell scripts inside powershell folder

- `Add-Cert-Manager.ps1`: Adds cert manager to AKS.
- `Build-Push.ps1`: Build and pushes all Docker images to ACR.
- `Create-Secret.ps1`: Create the secret `acr-auth` in AKS used to allow pulling images from ACR.
- `Deploy-Arm-Azure.ps1`: Deploys the ARM script to Azure and configure resources.
- `Deploy-Images-Aks.ps1`: Deploys Tailwind Traders Backend Docker images to AKS using Helm charts.
- `Deploy-Pictures-Azure.ps1`: Deploys all pictures to the Azure storage.
- `Enable-Ssl.ps1`: Enables SSL suport in AKS using cert-manager.
- `Generate-Config.ps1`: Generates a valid configuration file given an Azure resource group with all infrastructure created.
- `Token-Replace.ps1`: Used by `Generate-Config.ps1`. Substitutes tokens in a template file by values given.

## ARM Scripts inside arm folder

- `deployment.json`: Full ARM script with AKS, ACR and all infrastructure in Azure
- `deployment-nodes.json`: ARM script with an AKS that allows Windows and Linux containers.
- `deployment-no-aks.json`: ARM script with all infrastructure but without AKS nor ACR. Used when using an existing AKS and ACR.

# Deploy folder

This folder contains a set of deployment scripts to cover various scenarios

## Powershell scripts

* `Add-Cert-Manager.ps1`: Adds cert manager to AKS. Described in the [Deployment Backend to AKS doc](../Documents/DeploymentGuide.md)
* `Add-Tiller.ps1`: Installs and configure Tiller. Described in the [Deployment Backend to AKS doc](../Documents/DeploymentGuide.md)
* `Build-Push.ps1`: Build and pushes all Docker images to ACR. Described in the [Deployment Backend to AKS doc](../Documents/DeploymentGuide.md) 
* `Create-Secret.ps1`: Create the secret `acr-auth` in AKS used to allow pulling images from ACR. Described in the [Deployment Backend to AKS doc](../Documents/DeploymentGuide.md) 
* `Deploy-Arm-Azure.ps1`: Deploys the ARM script to Azure and configure resources. Described in the [Deploying Azure resources doc](../Documents/Azure-Deployment.md) 
* `Deploy-CosmosDb-Aci.ps1`: Deploys the CosmosDb emulator in ACI. Described in the [Deployment of all infrastructure in AKS](../Documents/AKS-infrastructure.md)
* `Deploy-Images-Aks.ps1`: Deploys Tailwind Traders Backend Docker images to AKS using Helm charts. Described in the [Deployment Backend to AKS doc](../Documents/DeploymentGuide.md)
* ` Deploy-Pictures-Aks.ps1`: Deploys all pictures to the Azure storage emulator running in AKS. Described in the [Deployment of all infrastructure in AKS](../Documents/AKS-infrastructure.md)
* `Deploy-Pictures-Azure.ps1`: Deploys all pictures to the Azure storage. Described in the [Deployment Backend to AKS doc](../Documents/DeploymentGuide.md)
* `Enable-Ssl.ps1`: Enables SSL suport in AKS using cert-manager. Described in the [Deployment Backend to AKS doc](../Documents/DeploymentGuide.md)
* `Generate-Config.ps1`: Generates a valid configuration file given an Azure resource group with all infrastructure created. Described in the [Deployment Backend to AKS doc](../Documents/DeploymentGuide.md)
* `token-replace.ps1`: Used by `Generate-Config.ps1`. Substitutes tokens in a template file by values given.

## ARM Scripts

* `deployment.json`: Full ARM script with AKS, ACR and all infrastructure in Azure
* `deployment-only-inf.json`: ARM script with only AKS and ACR. Used when infrastructure runs in AKS instead of Azure
* `deployment-no-aks.json`: ARM script with all infrastructure but without AKS nor ACR. Used when using an existing AKS and ACR.
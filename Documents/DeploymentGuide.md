# Deploy Backend services on AKS

Pre-requisites for this deployment are to have 

* The AKS and all related resources deployed in Azure  
* A terminal with
    * Bash environment with [jq](https://stedolan.github.io/jq/) installed **-OR-**
    * Powershell environment
* [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) installed.
* [Kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/) installed.
* Docker installed

**Note**: The easiest way to have a working Bash environment on Windows is [enabling the WSL](https://docs.microsoft.com/en-us/windows/wsl/install-win10) and installing a Linux distro from the Windows Store.

## Connecting kubectl to AKS

From the terminal type:

* `az login` and follow instructions to log into your Azure.
* If you have more than one subscription type `az account list -o table` to list all your Azure subscriptions. Then type  `az account set --subscription <subscription-id>` to select your subscription
* `az aks get-credentials -n <your-aks-name> -g <resource-group-name>` to download the configuration files that `kubectl` needs to connect to your AKS.

At this point if you type `kubectl config current-context` the name of your AKS cluster should be displayed. That means that `kubectl` is ready to use your AKS

## Installing Tiller on AKS

Helm is a tool to deploy resources in a Kubernetes cluster in a clean and simple manner. It is composed of two tools, one client-side (the Helm client) that needs to be installed on your machine, and a server component called _Tiller_ that has to be installed on the Kubernetes cluster.

To install Helm, refer to its [installation page](https://docs.helm.sh/using_helm/#installing-helm). Once Helm is installed, _Tiller_ must be deployed on the cluster. For deploying _Tiller_ run the `add-tiller.sh` (from Bash) or the `Add-Tiller.ps1` (from Powershell).

Once installed, helm commands like `helm ls` should work without any error.

## Configuring services

Before deploying services using Helm, you need to setup the configuration by editing the file `helm/gvalues.yaml` and put the secrets, connection strings and all the configuration.

Please refer to the comments of the file for its usage. Just ignore (but not delete) the `tls` section as, currently, TLS is not supported (its on the roadmap).

## Create secrets on the AKS

Docker images are stored in a ACR (a private Docker Registry hosted in Azure).

Before deploying anything on AKS, a secret must be installed to allow AKS to connect to the ACR through a Kubernetes' service account. 

To do so from a Bash terminal run the file `./create-secret.sh` with following parameters:

* `-g <group>` Resource group where AKS is
* `--acr-name <name>`  Name of the ACR
* `--clientid <id>` Client id of the service principal to use
* `--password <pwd>` Service principal password

If using Powershell run the `./Create-Secret.ps1` with following parameters:

* `-resourceGroup <group>` Resource group where AKS is
* `-acrName <name>`  Name of the ACR
* `-clientId <id>` Client id of the service principal to use
* `-password <pwd>` Service principal password

Please, note that the Service principal must be already exist. To create a service principal you can run the command `az ad sp create-for-rbac`.

## Build & deploy images to ACR

Run `docker-compose build` and then manually tag and push the images to your ACR.

**Note**: Under WSL docker daemon do not run, so you won't be able to use `docker-compose build` unless you configure docker client to use the Docker CE for Windows daemon. You can always build docker images using Docker CE for Windows, from a Windows command prompt, and run all scripts from WSL.

## Deploying services

To deploy the services from a Bash terminal run the `./deploy-images-aks.sh` script with the following parameters:

* `-n <name>` Name of the deployment. Defaults to  `my-tt`
* `--aks-name <name>` Name of the AKS
* `-g <group>` Name of the resource group
* `--acr-name <name>` Name of the ACR
* `--tag <tag>` Docker images tag to use. Defaults to  `latest`
* `--charts <charts>` List of comma-separated values with charts to install. Defaults to `*` (all)

If using Powershell, have to run `./Deploy-Images-Aks.ps1` with following parameters:

* `-name <name>` Name of the deployment. Defaults to  `my-tt`
* `-aksName <name>` Name of the AKS
* `-resourceGroup <group>` Name of the resource group
* `-acrName <name>` Name of the ACR
* `-tag <tag>` Docker images tag to use. Defaults to  `latest`
* `-charts <charts>` List of comma-separated values with charts to install. Defaults to `*` (all)

This script will install all services using Helm and your custom configuration from file `gvalues.yaml`

The parameter `charts` allow for a selective installation of charts. Is a list of comma-separated values that mandates the services to deploy in the AKS. Values are:

* `pr` Products API
* `cp` Coupons API
* `pf` Profiles API
* `pp` Popular products API
* `st` Stock API
* `ic` Image classifier API
* `ct` Shopping cart API
* `mgw` Mobile Api Gateway
* `wgw` Web Api Gateway

So, using `charts pp,st` will only install the popular products and the stock api.

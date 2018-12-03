# Deploy Backend services on AKS

Pre-requisites for this deployment are to have the AKS created in Azure and a Bash environment with [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) and [jq](https://stedolan.github.io/jq/) installed.

Also `kubectl` tool must be installed and configured to use the AKS context.

## Installing Tiller on AKS

Before installing the Backend services using Helm, Tiller must be installed. For installing Tiller run the `add-tiller.sh` file from terminal.

## Configuring services

Before deploying services using Helm, you need to setup the configuration by editing the file `helm/gvalues.yaml` and put the secrets, connection strings and all the configuration.

Please refer to the comments of the file for its usage.

## Create secrets on the AKS

Before deploying anything on AKS, a secret must be installed to allow AKS to connect to ACR through a Kubernetes' service account. To do so, run the file `./create-secret.sh` with following parameters:

* `-g <group>` Resource group where AKS is
* `--acr-name <name>`  Name of the ACR
* `--clientid <id>` Client id of the service principal to use
* `--password <pwd>` Service principal password

Please, note that the Service principal must be already exist

## Build & deploy images to ACR

Run `docker-compose build` and then manually tag and push the images to your ACR.

## Deploying services

To deploy the services run the `./deploy-images-aks.sh` script with the following parameters:

* `-n <name>` Name of the deployment. Defaults to  `my-tt`
* `--aks-name <name>` Name of the AKS
* `-g <group>` Name of the resource group
* `--acr-name <name>` Name of the ACR
* `--tag <tag>` Docker images tag to use. Defaults to  `c2018`

This script will install all services using Helm and your custom configuration from file `gvalues.yaml`
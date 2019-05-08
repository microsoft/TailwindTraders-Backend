# Deploying infrastructure on AKS

For rapid development/test scenarios could be useful to deploy the infrastructure on AKS instead of creating Azure resources:

## Pre-requisites

Same pre-requisites as the [standard AKS deployment](./DeploymentGuide.md)

## Configure the cluster (insalling Helm and the secrets)

You have to follow the following steps of the [standard AKS deployment](./DeploymentGuide.md):

* Connecting kubectl to AKS
* Installing Tiller on AKS
* Create secrets on the AKS
* Build & deploy images to ACR

You can skip the step "Configuring services" because there is no need to configure anything.

## Creating the ACI resource

Some of the Backend services use a CosmosDb resource. The CosmosDb emulator will be used in order to avoid creating a real CosmosDb account. Currently the emulator do not run under Linux containers, so it is deployed in a Azure Container Instance. To create the ACI you have to run the `Deploy-CosmosDb-Aci.ps1` with following parameters:

* `-resourceGroup`: Resource group where to create the ACI
* -`name`: Name of the ACI

This will create the ACI resource and deploy the Azure CosmosDb emulator image running on it.

## Deploying mongodb, sql server and azurite (storage emulator) on AKS

Deploy the infrastructure alongside the Backend is done with the same script used to deploy the Backend: `Deploy-Images-Aks.ps1`. You have to pass the parameter `-useInfrainAKS` to `$true`. Passing this parameter will:

1. Configure the services to use the SQL Server, MongoDb and storage emulator deployed as a containers in the same AKS
2. Ignore the value passed in the `-valuesFile` parameter (the `gvalues_inf.yaml` which contains needed values is used instead).
3. Force to you to pass also the parameters `-cartAciGroup` and `-cartAciName` with the Resource Group and name where the ACI running the CosmosDb emulator is.

The parameter `-useInfrainAKS` won't deploy the infrastructure in the AKS. **This is done by adding `infra` to the `-charts` parameter**. Note that the `infra` chart is only deployed if `-charts` contains the `infra` value. So if you want to deploy all services and the infrastructure must use `-charts="*,infra"` (`*` means "all backend services"). Refer to the "Deploying services" section in the [standard AKS deployment](./DeploymentGuide.md) for more information.

When `infra` value is used, three additional deployments are installed on the Kubernetes:

* One deployment to run a MongoDb
* One deployment to run a SQL Server
* One deployment to run [azurite](https://github.com/Azure/Azurite) (a lightweight linux-compatible storage emulator).

>**Note**: Azurite is exposed outside the cluster through an additional ingress on the `/blobs` endpoint. MongoDb and SQL Server are not exposed outside.

Assuming the images are pushed in the ACR, following commands will install **all Tailwind Traders Backend and infrastructure** in an AKS named `my-aks` in the RG `my-rg`, using images from ACR named `my-acr`. An ACI named `my-aci-tt` will be created in the same RG to run the CosmosDb emulator:

```
.\Deploy-CosmosDb-Aci.ps1 -resourceGroup my-rg -name my-aci-tt
.\Deploy-Images-Aks.ps1 -aksName my-aks -resourceGroup my-rg -acrName my-acr -useInfraInAks $true -cartAciGroup my-rg -cartAciName my-aci-tt  -charts "*,infra"
```

If you prefer you can deploy only the infrastructure first:

```
 .\Deploy-Images-Aks.ps1 -aksName my-aks -resourceGroup my-rg -acrName my-acr -charts "infra"
```

And deploy later just the backend services:

```
.\Deploy-Images-Aks.ps1 -aksName my-aks -resourceGroup my-rg -acrName my-acr -useInfraInAks $true -cartAciGroup my-rg -cartAciName my-aci-tt -charts "*"
```

## Deploying the images on the storage

To deploy the needed images on the Azurite running in the AKS just run the `/Deploy/Deploy-Pictures-Aks.ps1` script, with following parameters:

* `-resourceGroup <name>`: Resource group where storage is created
* `-aksName <name>`: Name of the AKS

Script will create blob containers and copy the images (located in `/Deploy/tt-images` folder) to the storage account.

>**Note** Azurite must be up and running in the AKS for the script to run.

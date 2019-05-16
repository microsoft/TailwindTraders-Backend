# Tailwind Traders: Demoscript for Virtual Nodes

The goal of this demo is to view the AKS "virtual nodes" feature, that enables running some AKS pods in ACI

## Key Takeaway

Virtual Nodes is a key feature of AKS that run some pods in ACI to allow for high scalability in scenarios where scalability can vary a lot. This demo starts with a "standard" deploy of Tailwind Traders Backend, and then this deploy is updated to allow some APIs to run on virtual nodes.

## Before you begin

You will need:

- Azure CLI installed

## Pre-setup steps

Those steps need to be done once before the demo:

### Create the AKS 

The [ARM script](../../../Deploy/deployment.json) provided with Tailwind Traders is not configured to create an AKS with virtual nodes feature enabled, and as this feature can'be added to an AKS after its creation, **you will need to create an AKS with virtual nodes enabled**. You can:

* [Use Azure portal to create an AKS with virtual nodes enabled](https://docs.microsoft.com/en-us/azure/aks/virtual-nodes-portal)
* [Use the CLI to create an AKS with virtual nodes enabled](https://docs.microsoft.com/en-us/azure/aks/virtual-nodes-cli)
* Run the powershell script `/Deploy/vnodes/Create-Aks.ps1`.

The powershell script has following parameters:

* `-resourceGroup`: Resource group to use. If not exists will be created
* `-location`: Location where to create the resource group if needed. Defaults to `eastus2`
* `-aksName`: Name of the AKS cluster to create. Defaults to `ttvnodes`

### Create an ACR

Type following command to create an ACR:

```
az acr create -g <resource-group> -n <acr-name> --admin-enabled true --sku Standard
```

### Deploy Azure infrastructure

Run the `/Deploy/Deploy-Arm-Azure.ps1` script with following parameters:

* `-resourceGroup`: Resource group where to deploy all Azure infrastructure. If not exists it is created.
* `-location`: Location where create the resource group if needed
* -`-deployAks`: Set it to `$false` to NOT create the AKS.



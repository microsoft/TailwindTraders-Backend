# Tailwind Traders Azure Resources Deployment

To run Tailwind Traders you need to create the Azure infrastructure. There are two ways to do it. Using Azure portal or using a Powershell script.

## Creating infrastructure using Azure Portal

An ARM script is provided that can be deployed just clicking following button:

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FTailwindTraders-Backend%2Fmaster%2FDeploy%2Fdeployment.json"><img src="./Images/deploy-to-azure.png" alt="Deploy to Azure"/></a>

Azure portal will ask you for the following parameters:

* `servicePrincipalId`: Id of the service principal used to create the AKS
* `servicePrincipalSecret`: Password of the service principal
* `sqlServerAdministratorLogin`: Name of the user for the databases
* `sqlServerAdministratorLoginPassword`: Password for the user of the databases
* `aksVersion`: AKS version to use.
* `pgversion`: Version of the Azure database for PostgreSQL to install. Defaults to `10`.

The deployment could take more than 10 minutes, and once finished all needed resources will be created:

![Resource group with all azure resources created](./Images/azure-resources.png)

### Creating the stockdb database in PostgreSQL

Once the deployment is finished (it could take more than 10 minutes) an additional step has to be done: You need to create a database named `stockdb` in the PostgreSQL server. For this you need the **[Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) installed**.

Just type the command:

```
az postgres db create -g <resource-group> -s <posgres-server-name> -n stockdb
```

## Create the resources using the CLI

You can use the CLI to deploy the ARM script. Open a Powershell window from the `/Deploy` folder and run the `Deploy-Arm-Azure.ps1` with following parameters:

* `-resourceGroup`: Name of the resource group
* `-location`: Location of the resource group

You can optionally pass two additional parameters:

* `-clientId`: Id of the service principal uesd to create the AKS
* `-password`: Password of the service principal 

If these two parameters are not passed a new service principal will be created.

There are three additional optional parameters to control some aspects of what is created:

* `-dbAdmin`: Name of the user of all databases. Defaults to `ttadmin`
* `-dbPassword`: Passwowrd of the user of all databases. Defaults to `Passw0rd1!`
* `-deployAks`: If set to `$false` AKS and ACR are not created. This is useful if you want to create the AKS yourself or use an existing AKS. Defaults to `$true`. If this parameter is `$true` the resource group can't exist (AKS must be deployed in a new resource group).

Once script finishes, everything is installed. If a service principal has been created, the script will output the service principal details.

## Install the Tailwind Traders Backend on the AKS

Now you are ready to install the backend on the AKS. Please follow the [guideline on how to do it](./DeploymentGuide.md).
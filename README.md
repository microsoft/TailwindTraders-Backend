# Deploy to Azure

We have added an ARM template so you can automate the creation of the resources required for the backend services.

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FTailwindTraders-Backend%2Fmaster%2FDeploy%2Fdeployment.json"><img src="/Documents/Images/deploy-to-azure.png" alt="Deploy to Azure"/></a>

> Note: This deployment can take up to 12 minutes.

Please, note that **this only deploys the needed infrastructure**. You need to deploy the services to your infrastructure, following these [instructions](./Documents/DeploymentGuide.md)

* To run the backend locally on your computer follow these [instructions](./Documents/RunLocally.md)

[![Build status](https://dev.azure.com/TailwindTraders/Backend/_apis/build/status/Backend-CI)](https://dev.azure.com/TailwindTraders/Backend/_build/latest?definitionId=26)

## Running everything on AKS

For development scenarios everything can be run on a AKS, so **not external dependencies needed**. Click following button to deploy only an AKS:
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FTailwindTraders-Backend%2Fmaster%2FDeploy%2Fdeployment-only-inf.json"><img src="/Documents/Images/deploy-to-azure.png" alt="Deploy to Azure"/></a>

Once you have an AKS please follow the documentation to [deploy infrastructure on AKS](./Documents/AKS-infrastructure.md)

# Running the backend services

Please refer to the [deployment guide](Documents/DeploymentGuide.md) for the required steps to run the backend services.

# Test image classiffier

Please refer to the [test image classiffier guide](Documents/TestImageClassiffierGuide.md).

# Repositories

For this demo reference, we built several consumer and line-of-business applications and a set of backend services. You can find all repositories in the following locations:

* [Tailwind Traders](https://github.com/Microsoft/TailwindTraders)
* [Backend (AKS)](https://github.com/Microsoft/TailwindTraders-Backend)
* [Website (ASP.NET & React)](https://github.com/Microsoft/TailwindTraders-Website)
* [Desktop (WinForms & WPF -.NET Core)](https://github.com/Microsoft/TailwindTraders-Desktop)
* [Rewards (ASP.NET Framework)](https://github.com/Microsoft/TailwindTraders-Rewards)
* [Mobile (Xamarin Forms 4.0)](https://github.com/Microsoft/TailwindTraders-Mobile)



# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

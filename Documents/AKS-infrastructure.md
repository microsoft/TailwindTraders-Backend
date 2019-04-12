# Deploying infrastructure on AKS

For rapid development/test scenarios could be useful to deploy the infrastructure on AKS. 

## Pre-requisites

* An AKS installed
* `kubectl` configured against this AKS
* `helm` installed and _tiller_ configured in AKS
* Powershell

All steps are done in a _powershell_ window located in folder `/Deploy`.

## Deploying mongodb, sql server and azurite (storage emulator) on AKS

The script `Deploy-Infra-Aks.ps1` will deploy all needed infrastructure on the AKS:


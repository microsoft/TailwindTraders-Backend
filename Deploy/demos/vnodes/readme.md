# Deploy/demos/vnodes folder

This folder contains additional scripts needed for testing the vnodes scenario


## Pre steps

1. Install prometheus operator: `kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/prometheus-operator/master/bundle.yaml`
2. Run prometheus instance: `kubectl apply -f yamls`
3. Deploy Prometheus Metrics Adapter:
    * `helm repo add prometheus-community https://prometheus-community.github.io/helm-charts`
    * `helm repo update`
    * `helm install prometheus-adapter prometheus-community/prometheus-adapter  -f ./prometheus-adapter/values.yaml`
4. Deploy the service monitor to monitor products api: `kubectl apply -f service-monitor`
5. Create the HPA for products: `kubectl apply -f hpa`

## Powershell scripts

- `Create-Aks.ps1`: Creates an AKS with vnodes enabled.

#! /usr/bin/pwsh

# Installs cert-manager on cluster
Invoke-Expression "helm install --name cert-manager --namespace kube-system  --version v0.4.1 stable/cert-manager"
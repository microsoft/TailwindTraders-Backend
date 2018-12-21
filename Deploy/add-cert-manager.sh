#!/bin/sh

# Installs cert-manager on cluster
helm install --name cert-manager --namespace kube-system  --version v0.4.1  stable/cert-manager
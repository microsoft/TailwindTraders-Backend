#! /usr/bin/pwsh

Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Configuring RBAC for Tiller" -ForegroundColor Yellow
Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
kubectl create serviceaccount --namespace kube-system tiller
kubectl create clusterrolebinding tiller-cluster-rule --clusterrole=cluster-admin --serviceaccount=kube-system:tiller
Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Installing Helm" -ForegroundColor Yellow
Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
helm list -q  | Out-Null
if ($?) {
    helm init -c --service-account tiller --node-selectors "kubernetes.io/os=linux"
}
else {
    helm init --service-account tiller --node-selectors "kubernetes.io/os=linux" --wait
}

helm list -q  | Out-Null
exit $LastExitCode
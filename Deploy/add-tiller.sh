echo "------------------------------------------------------------"
echo "Configuring RBAC for Tiller"
echo "------------------------------------------------------------"
kubectl create serviceaccount --namespace kube-system tiller
kubectl create clusterrolebinding tiller-cluster-rule --clusterrole=cluster-admin --serviceaccount=kube-system:tiller
kubectl patch deploy --namespace kube-system tiller-deploy -p '{"spec":{"template":{"spec":{"serviceAccount":"tiller"}}}}'      
echo "------------------------------------------------------------"
echo "Installing Helm"
echo "------------------------------------------------------------"
helm init --service-account tiller

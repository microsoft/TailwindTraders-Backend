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

## Verify metrics server

Verify metrics are exposed through metrics server: `kubectl get --raw /apis/custom.metrics.k8s.io/v1beta1/ | jq` 

```json
{
  "kind": "APIResourceList",
  "apiVersion": "v1",
  "groupVersion": "custom.metrics.k8s.io/v1beta1",
  "resources": [
    {
      "name": "namespaces/http_requests_in_progress",
      "singularName": "",
      "namespaced": false,
      "kind": "MetricValueList",
      "verbs": [
        "get"
      ]
    },
    {
      "name": "pods/http_requests_in_progress",
      "singularName": "",
      "namespaced": true,
      "kind": "MetricValueList",
      "verbs": [
        "get"
      ]
    }
  ]
}
```

Verify metrics can be fetched from Prometheus: `kubectl get --raw /apis/custom.metrics.k8s.io/v1beta1/namespace/default/http_requests_in_progress`

```json
{
  "kind": "MetricValueList",
  "apiVersion": "custom.metrics.k8s.io/v1beta1",
  "metadata": {
    "selfLink": "/apis/custom.metrics.k8s.io/v1beta1/namespace/default/http_requests_in_progress"
  },
  "items": [
    {
      "describedObject": {
        "kind": "Namespace",
        "name": "default",
        "apiVersion": "/v1"
      },
      "metricName": "http_requests_in_progress",
      "timestamp": "2020-10-09T09:02:36Z",
      "value": "1",
      "selector": null
    }
  ]
}
```

## Run blackfriday scripts

Now we need to do some calls to the products API. You can use the  blackfriday script. Run several of them (`./blackfriday.sh -d <dns-ingress> /dev/null &`) and see how metric is growing in the HPA (`kubectl get hpa kubectl get hpa products-reqs-progress-hpa -w`).

Once demo is finished kill the blackfriday scripts:

`kill $(ps | grep blackfriday.sh |  awk -F" " '{print $1}' )`

After some time HPA will scale down the replicas.
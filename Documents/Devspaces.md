# Support for Azure Devspaces

Tailwind Traders supports [Azure Devspaces](https://docs.microsoft.com/en-us/azure/dev-spaces/). Follow the steps in this document to deploy Tailwind traders under devspaces.

## Requeriments

* AKS with Devspaces enabled
* Devspaces CLI installed

**Note** Tailwind Traders has been tested with Devspaces CLI version:

```
Azure Dev Spaces CLI
1.0.20190423.8
API v3.2 
```

## Creating a parent Devspace

First you need to create the parent devspace, using Azure CLI:

```
> azds space select
Select a dev space or Kubernetes namespace to use as a dev space.
 [1] default
Type a number or a new name:
```  

Type the name of the parent devspace in the prompr (like dev):

```
Dev space 'dev' does not exist and will be created.

Select a parent dev space or Kubernetes namespace to use as a parent dev space.
 [0] <none>
 [1] default
Type a number:
```

Type `0` to make the `dev` devspace a root devspace.

Then the devspace is created. You can check that the devspace is created by typing:

```
>azds space list
   Name     DevSpacesEnabled
-  -------  ----------------
   default  False
*  dev      True
```

## Deploying the serviceaccount in the namespace

All pods created by Helm charts run under the `ttsa` service account. You **must deploy the service account before deploying any DevSpaces workload**. Just apply the file `/Deploy/helm/ttsa.yaml` on the Devspace namespace (i. e. `dev`):

```
kubectl apply -f <path/to/deploy/helm/ttsa.yaml> -n dev
```

## Deploying to the parent Devspace using CLI

Like deploying without devspaces you need a configuration file (a _gvalues.yml_ like file) with all the needed configuration (connection strings, storage keys, endpoints, etc). To be used by devspaces this file **has to be named `gvalues.azds.yaml`** and **has to be located in the `/Deploy/helm/` folder**.

>**Note**: File `/Deploy/helm/gvalues.azds.yaml` is in the `.gitignore`, so it is ignored by Git.

You should have to copy your configuration file to the `/Deploy/helm` and rename to `gvalues.azds.yaml`. The powershell script `/Source/prepare-devspaces.ps1` can do it for you:

```
.\prepare-devspaces.ps1 -file \Path\To\My\Config\File.yml
```

The script just copies the file passed in to the `/Deploy/helm` folder with the right name. If file already exists is overwritted.

Once you have a valid configuration file, you need to deploy the APIs to the devspaces. You need to go to the **root source folder of each API** and type:

```
azds up -v -d
```

(The _root source folder_ of each API is the one that has the `azds.yaml` file, like `/Source/Services/Tailwind.Traders.Login.Api/` or `/Source/ApiGWs/Tailwind.Traders.WebBff/`).

APIs that have devspaces enabled are:

* MobileBFF (`/Source/ApiGWs/Tailwind.Traders.Bff`) - a Net Core API
* WebBFF (`/Source/ApiGWs/Tailwind.Traders.WebBff`) - a Net Core API
* Cart API (`/Source/Services/Tailwind.Traders.Cart.Api`) - a Node.js API
* Coupons API (`/Source/Services/Tailwind.Traders.Coupon.Api`) - a Node.js API
* Login API (`/Source/Services/Tailwind.Traders.Login.Api`) - a Net Core API
* Popular Products API (`/Source/Services/Tailwind.Traders.PopularProduct.Api`) - a Golang API
* Profiles API (`/Source/Services/Tailwind.Traders.Profile.Api`) - a Net Core API
* Stock API (`/Source/Services/Tailwind.Traders.Stock.Api`) - a Java API

Once you have all them deployed in Dev Spaces you can check it using `azds list-up`:

```
>  azds list-up
Name             DevSpace  Type     Updated  Status
---------------  --------  -------  -------  -------
cart             dev       Service  8m ago   Running
coupons          dev       Service  7m ago   Running
login            dev       Service  7m ago   Running
mobilebff        dev       Service  15m ago  Running
popularproducts  dev       Service  5m ago   Running
product          dev       Service  3m ago   Running
profile          dev       Service  2m ago   Running
stock            dev       Service  1m ago   Running
webbff           dev       Service  9m ago   Running
```

Each API has its own _ingress_ created. The command `azds list-uris` will display all URIs for every service:

```
>  azds list-uris
Uri                                                                     Status
----------------------------------------------------------------------  ---------
http://dev.tt.xxxxxxxxxs.weu.azds.io/cart-api                           Available
http://dev.tt.xxxxxxxxxs.weu.azds.io/coupons-api                        Available
http://dev.tt.xxxxxxxxxs.weu.azds.io/login-api                          Available
http://dev.tt.xxxxxxxxxs.weu.azds.io/mobilebff                          Available
http://dev.tt.xxxxxxxxxs.weu.azds.io/popular-products-api               Available
http://dev.tt.xxxxxxxxxs.weu.azds.io/product-api                        Available
http://dev.tt.xxxxxxxxxs.weu.azds.io/profile-api                        Available
http://dev.tt.xxxxxxxxxs.weu.azds.io/stock-api                          Available
http://dev.tt.xxxxxxxxxs.weu.azds.io/webbff                             Available
```

All pods run in the namespace selected as a Dev Space (`dev` in our case). Using `kubectl get pods` will show all running pods:

```
>  kubectl get pods -n dev
NAME                                                  READY   STATUS    RESTARTS   AGE
azds-14223a-dev-tt-popularproducts-79667b6684-75vd8   2/2     Running   0          6m42s
azds-212ef9-dev-tt-products-59f77b8bb6-rbql9          2/2     Running   0          4m2s
azds-26b908-dev-tt-profile-7f97b8d5cb-lwq2k           2/2     Running   0          2m57s
azds-3128d4-dev-tt-login-5b976cf44b-zjkp7             2/2     Running   0          7m59s
azds-312ebd-dev-tt-stock-7b6fb8b87f-wvq62             2/2     Running   0          114s
azds-6ab0d6-dev-tt-cart-574bbf95fb-rvshq              2/2     Running   0          9m17s
azds-70a6e6-dev-tt-coupons-775d47fcf7-rcfq9           2/2     Running   0          8m46s
azds-c16cb7-dev-tt-webbff-cc899c886-sb9bx             2/2     Running   0          10m
azds-c9f588-dev-tt-mobilebff-776cc9f45f-drgdz         2/2     Running   0          13m
```

**Congratulations!** You have deployed all APIs in a parent Dev Space

## Deploying on a child Dev Space

To deploy an API to a child Dev Space just create the child Dev Space (using `azds space select`), selecting `dev` as parent devspace. Then deploy again (with `azds up -d -v`) one of the APIs.

Here Alice is one engineer that need to fix a bug in the _Stock API_, so first she creates a child devspace for her to use:

```
>  azds space select
Select a dev space or Kubernetes namespace to use as a dev space.
 [1] default
 [2] dev
Type a number or a new name: alice

Dev space 'alice' does not exist and will be created.

Select a parent dev space or Kubernetes namespace to use as a parent dev space.
 [0] <none>
 [1] default
 [2] dev
Type a number: 2

Creating and selecting dev space 'dev/alice'...2s
```

An `azds space list` verifies that she is using the Dev Space `alice`:

```
>  azds space list
   Name       DevSpacesEnabled
-  ---------  ----------------
   default    False
   dev        True
*  dev/alice  True
```

Fist **she must deploy the `/Deploy/helm/ttsa.yaml` file using `kubectl apply`**. If she fails doing that, the Devspaces deploy will be stuck at "Waiting for container image build..." phase.

She can now deploy her version of Stock API, just going to `/Source/Services/Tailwind.Traders.Stock.Api` and use `azds up -d -v`. Once service is deployed a `azds list-up` shows that, for Alice, all APIs runs on `dev` but _Stock API_:

```
>  azds list-up
Name             DevSpace  Type     Updated  Status
---------------  --------  -------  -------  -------
cart             dev       Service  16m ago  Running
coupons          dev       Service  16m ago  Running
login            dev       Service  15m ago  Running
mobilebff        dev       Service  23m ago  Running
popularproducts  dev       Service  13m ago  Running
product          dev       Service  11m ago  Running
profile          dev       Service  10m ago  Running
stock            alice     Service  1m ago   Running
webbff           dev       Service  17m ago  Running
```

If Alice types `azds list-uris` she will see the URIs for her namespace. These are the uris she has to use:

```
> azds list-uris
Uri                                                                             Status
------------------------------------------------------------------------------  ---------
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/cart-api                           Available
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/coupons-api                        Available
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/login-api                          Available
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/mobilebff                          Available
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/popular-products-api               Available
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/product-api                        Available
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/profile-api                        Available
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/stock-api                          Available
http://alice.s.dev.tt.xxxxxxxxxs.weu.azds.io/webbff                             Available
```

Next step is [deploy the website in the devspaces](https://github.com/Microsoft/TailwindTraders-Website/blob/master/Documents/Devspaces.md) too.

>**Note**: The web **must be** deployed in the same AKS that Backend is deployed. Deploy 1st the backend and then the Website.
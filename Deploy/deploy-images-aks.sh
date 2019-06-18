name="my-tt"
aksName=${TT_AKS_NAME}
aksRg=${TT_AKS_RG}
acrName=${TT_ACR_NAME}
aksHost=""
acrLogin=""
tag="latest"
charts="*"
valuesFile=gvalues.yaml

function validate {
    valid=1

    if [[ "$aksName" == "" ]] 
    then
        echo "No AKS name. Use --aks-name to specify name"
        valid=0
    fi
    if [[ "$aksRg" == "" ]] 
    then
        echo "No resource group. Use -g to specify resource group."
        valid=0
    fi    
    if [[ "$aksHost" == "" ]] 
    then
        echo "AKS host of HttpRouting can't be found. Are you using right AKS ($aksName) and RG ($aksRg)?"
        valid=0
    fi     
    if [[ "$acrLogin" == "" ]] 
    then
        echo "ACR login server can't be found. Are you using right ACR ($acrName) and RG ($aksRg)?"
        valid=0
    fi               
    if (( valid == 0)) 
    then
        exit 1
    fi
}

while [ "$1" != "" ]; do
    case $1 in
        -n | --name)                    shift
                                        name=$1
                                        ;;
        -g | --resource-group)          shift
                                        aksRg=$1
                                        ;;
        --aks-name)                     shift
                                        aksName=$1
                                        ;;        
        --acr-name)                     shift
                                        acrName=$1
                                        ;;        
        --tag)                          shift
                                        tag=$1
                                        ;;
        --charts)                       shift
                                        charts=$1
                                        ;;
        -f | --values)                  shift
                                        valuesFile=$1
                                        ;;
       * )                              echo "Invalid param: $1"
                                        exit 1
    esac
    shift
done


echo --------------------------------------------------------
echo Deploying images on cluster $aksName
echo 
echo Additional parameters are:
echo Release Name: $name
echo AKS to use: $aksName in RG $aksRg and ACR $acrName
echo Images tag: $tag
echo --------------------------------------------------------

acrLogin=$(az acr show -n $acrName -g $aksRg -o json | jq ".loginServer" | tr -d '"')
aksHost=$(az aks show -n $aksName -g $aksRg -o json | jq ".addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName" | tr -d '"')

echo "acr login server is $acrLogin"
echo "aksHost is $aksHost"

validate

pushd helm

echo Deploying charts "$charts"

if [[ "$charts" == *"pr"* ]]  || [[ "$charts" == "*" ]]
then
    echo "Products chart - pr"
    helm install --name $name-product -f $valuesFile --set az.productvisitsurl=$afHost --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/product.api --set image.tag=$tag  products-api
fi

if [[ "$charts" == *"cp"* ]]  || [[ "$charts" == "*" ]]
then
    echo "Coupons chart - cp"
    helm install --name $name-coupon -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/coupon.api --set image.tag=$tag coupons-api
fi

if [[ "$charts" == *"pf"* ]]  || [[ "$charts" == "*" ]]
then
    echo "Profile chart - pf "
    helm install --name $name-profile -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/profile.api --set image.tag=$tag profiles-api
fi

if [[ "$charts" == *"pp"* ]]  || [[ "$charts" == "*" ]]
then
    echo "Popular products chart - pp"
    helm install --name $name-popular-product -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/popular-product.api --set image.tag=$tag --set initImage.repository=$acrLogin/popular-product-seed.api  --set initImage.tag=$tag popular-products-api 
fi

if [[ "$charts" == *"st"* ]]  || [[ "$charts" == "*" ]]
then
    echo "Stock -st"
    helm  install --name $name-stock -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/stock.api --set image.tag=$tag stock-api
fi

if [[ "$charts" == *"ic"* ]]  || [[ "$charts" == "*" ]]
then
    echo "Image Classifier -ic"
    helm  install --name $name-image-classifier -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/image-classifier.api --set image.tag=$tag image-classifier-api
fi

if [[ "$charts" == *"ct"* ]]  || [[ "$charts" == "*" ]]
then
    echo "Cart (Basket) -ct"
    helm  install --name $name-cart -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/cart.api --set image.tag=$tag cart-api
fi

if [[ "$charts" == *"mgw"* ]]  || [[ "$charts" == "*" ]]
then
    echo "mobilebff -mgw"
    helm  install --name $name-mobilebff -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/mobileapigw --set image.tag=$tag mobilebff
fi

if [[ "$charts" == *"wgw"* ]]  || [[ "$charts" == "*" ]]
then
    echo "webbff -wgw"
    helm  install --name $name-webbff -f $valuesFile --set ingress.hosts={$aksHost} --set image.repository=$acrLogin/webapigw --set image.tag=$tag webbff
fi

popd

echo "Tailwind traders deployed on AKS"
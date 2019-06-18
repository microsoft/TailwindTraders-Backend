spnClientId=${TT_SPN_CLIENT_ID}
spnPw=${TT_SPN_PWD}
acrName=${TT_ACR_NAME}

function validate {
    valid=1

    if [[ "$aksRg" == "" ]] 
    then
        echo "No resource group. Use -g to specify resource group."
        valid=0
    fi    
    if [[ "$acrLogin" == "" ]] 
    then
        echo "ACR login server can't be found. Are you using right ACR ($acrName) and RG ($aksRg)?"
        valid=0
    fi               
    if [[ "$spnClientId" == "" ]] 
    then
        echo "No Client ID. Use --clientid to specify a Client ID"
        valid=0
    fi
    if [[ "$spnPw" == "" ]] 
    then
        echo "No Client ID Pwd. Use --password to specify a Client ID Password"
        valid=0
    fi
    if (( valid == 0)) 
    then
        exit 1
    fi
}

while [ "$1" != "" ]; do
    case $1 in
        -g | --resource-group)          shift
                                        aksRg=$1
                                        ;;
        --acr-name)                     shift
                                        acrName=$1
                                        ;;        
        --clientid)                     shift
                                        spnClientId=$1
                                        ;;
        --password)                     shift
                                        spnPw=$1
                                        ;;                                                                                                                
       * )                              echo "Invalid param: $1"
                                        exit 1
    esac
    shift
done

echo --------------------------------------------------------
echo Deploying secret for accessing ACR
echo 
echo Additional parameters are:
echo  ACR $acrName in RG $aksRg
echo Client Id: $spnClientId with pwd: $spnPw
echo --------------------------------------------------------

acrLogin=$(az acr show -n $acrName -g $aksRg -o json | jq ".loginServer" | tr -d '"')
acrId=$(az acr show -n $acrName -g $aksRg -o json | jq ".id" | tr -d '"')
validate

az role assignment create --assignee $spnClientId --scope $acrId --role reader
kubectl delete secret acr-auth
kubectl create secret docker-registry acr-auth --docker-server $acrLogin --docker-username $spnClientId --docker-password $spnPw --docker-email not@used.com

echo Deploying ServiceAccount ttsa
kubectl apply -f helm/ttsa.yaml
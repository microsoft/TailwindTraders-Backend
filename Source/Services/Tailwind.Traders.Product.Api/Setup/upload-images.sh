file=ProductItems.csv
storageName=${TT_STORAGE_NAME}
imagesPath="./product-images"
aksRg=${TT_AKS_RG}

function validate {
    valid=1

    if [[ "$storageName" == "" ]] 
    then
        echo "No storage name. Use -s to set storage name"
        valid=0
    fi    

    if [[ "$(which csvtool)" == "" ]] 
    then
        echo "csvtool not found and it is required"
        valid=0
    fi  

    if [[ "$aksRg" == "" ]] 
    then
        echo "No resource group. Use -g to specify resource group."
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
        -f | --file)                    shift
                                        file=$1
                                        ;;
        --images-path)                  shift
                                        imagesPath=$1
                                        ;;                                        
        -s | --storage-name)            shift
                                        storageName=$1
                                        ;;                                                                                
       * )                              echo "Invalid param: $1"
                                        exit 1
    esac
    shift
done

validate

imageNames=$(csvtool drop 1 $file | csvtool -z col 3 -)
constr=$(az storage account show-connection-string -g $aksRg -n $storageName | jq ".connectionString" | tr -d '"')

while read -r imageName; do
    echo uploading  "$imagesPath/$imageName"... 
    az storage blob upload -f "$imagesPath/$imageName" -c product-list  -n $imageName --connection-string $constr
done <<< "$imageNames"
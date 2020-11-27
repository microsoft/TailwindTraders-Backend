#!/bin/bash

dns=""

while [ "$1" != "" ]; do
    case $1 in
        -d | --dns)                     shift
                                        dns=$1
                                        ;;
       * )                              echo "Invalid param. Use -d (ingress DNS)"
                                        exit 1
    esac
    shift
done

if [ -z "$dns" ]; then
    echo "No public ingress DNS provided. Use -d to provide it (no http prefix)"
    exit 1
fi


while :
do 
    id=$(shuf -i 1-100 -n 1)
    if (($id > 50)); then
        echo "# Calling http:$dns/product-api/v1/product"
        curl -s "http://$dns/product-api/v1/product" > /dev/null 
    else
        echo "! Calling http:$dns/product-api/v1/product/$id"
        curl -s "http://$dns/product-api/v1/product/$id" > /dev/null
    fi
done
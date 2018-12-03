How to build and deploy an AzureML Image using this package
===========================================================

First you need a few files from the docker app. 

Assuming you are in the azureml subdirectory run the following:

copy ..\app\requirements.txt
copy ..\app\predict.py
copy ..\app\model.pb
copy ..\app\labels.txt

These 4 files along with the score.py file will make up the assets needed to create an AzureML image.

Using the Azure ML Command Line Interface you can create and deploy a service using the following steps.

If you haven't previously setup a Model Management account, please follow the instructions at https://docs.microsoft.com/en-us/azure/machine-learning/preview/deployment-setup-configuration

1. Create a manifest to describe the image creation.

az ml manifest create --manifest-name <your manifest name> -m model.pb -d labels.txt -d predict.py -r python -p requirements.txt -f score.py

When this runs you'll see the following output:

model.pb
Successfully registered model
Id: <model id>
More information: 'az ml model show -m <model id>'
Creating new driver at C:\Users\<user name>\AppData\Local\Temp\tmp3i9c498m.py
labels.txt
predict.py
score.py
Successfully created manifest
Id: <manifest id>
More information: 'az ml manifest show -i <manifest id>


2. Create an image from the manifest

az ml image create --manifest-id <manifest id> -n <name of your image>

When this runs you'll see the following output:

Creating image...................Done.
Image ID: <image id>
More details: 'az ml image show -i <image id>'
Usage information: 'az ml image usage -i <image id>'

3. Create a realtime service to serve your model

az ml service create realtime -n <name of service> --image-id <image id>

This will now create and deploy a service. When successful it will log some usage information that will show you how
to connect to and test the service.

For more information please see:
 https://docs.microsoft.com/en-us/azure/machine-learning/preview/model-management-service-deploy
 https://docs.microsoft.com/en-us/azure/machine-learning/preview/model-management-custom-container
 https://docs.microsoft.com/en-us/azure/machine-learning/preview/deploy-to-iot-edge-device
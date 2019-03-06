# Test image clasifier

To test the image classiffier service, you can use the curl to get the suggested products.

The modifier "-v" is for verbose mode.

* To use the web backend for frontend gateway:
    * curl YOUR_URL_OF_BACKEND/webbff/V1/products/imageclassifier -X POST -F "file=@C:\YOUR_PATH_AND_FILENAME_OF_PHOTO_TO_SEARCH"  -v

* To call directly to image classifier service:
    * curl YOUR_URL_OF_BACKEND/image-classifier-api/V1/imageclassifier -X POST -F "file=@C:\YOUR_PATH_AND_FILENAME_OF_PHOTO_TO_SEARCH.jpg"  -v

The response should be similar to:
[{"id":57,"name":"Yellow hard hat with tool bag pack","price":46.0,"imageUrl":"YOUR_URL_OF_STORAGE/images/product-list/59890052.jpg"}]* Connection #0 to host localhost left intact


You have sample images to test this feature in:
* [./Images/ImageClassiffier/](./Images/ImageClassiffier/)
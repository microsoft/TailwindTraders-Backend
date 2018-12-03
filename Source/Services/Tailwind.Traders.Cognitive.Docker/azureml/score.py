from predict import initialize, predict_url, predict_image
from PIL import Image
import numpy as np

import base64
import io
import json

# Azure ML model loader
def init():
    initialize()

# Helper to predict an image encoded as base64
def predict_image_base64(encoded_image):
    if encoded_image.startswith('b\''):
        encoded_image = encoded_image[2:-1]

    decoded_img = base64.b64decode(encoded_image.encode('utf-8'))
    img_buffer  = io.BytesIO(decoded_img)

    image = Image.open(io.BytesIO(decoded_img))
    return predict_image(image)

# Azure ML entry point
def run(json_input):
    try:
        results = None
        input = json.loads(json_input)
        url = input.get("url", None)
        image = input.get("image", None)

        if url:
            results = predict_url(url)
        elif image:
            results = predict_image_base64(image)
        else:
            raise Exception("Invalid input. Expected url or image")
        return (results)
    except Exception as e:
        return (str(e))

if __name__ == "__main__":
    init()

    # test image

    image = Image.open("test_image.jpg")
    imgio = io.BytesIO()
    image.save(imgio, "PNG")
    imgio.seek(0)
    dataimg = base64.b64encode(imgio.read())
    input = '{"image": "' + dataimg.decode('utf-8') + '"}'
    print ("calling")
    result = run(input)
    input_url = '{"url": "https://raw.githubusercontent.com/Microsoft/Cognitive-CustomVision-Windows/master/Samples/Images/Test/test_image.jpg" }'
    run(input_url)
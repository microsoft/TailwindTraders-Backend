FROM python:3.5

ADD app /app

RUN pip install --upgrade pip
RUN pip install -r /app/requirements.txt

# Expose the port
EXPOSE 80

# Set the working directory
WORKDIR /app

# Run the flask server for the endpoints
CMD python app.py
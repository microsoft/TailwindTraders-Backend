# Create the database
Popular products api provides information about the user preferred products.

## Manually

1. Go to `scripts` folder
2. Open Linux terminal and set following environment variables: `dbserver`, `dbuser`, `dbpassword` and `dbcatalog` with the name of server, user, password and database to create and fill
3. Run the file `run.sh`

## Using Docker

The file `Dockerfile.init` allows to create a Docker image that creates and fills the database:

```
docker build -t <name-of-image> --file Dockerfile.init .
docker run -e dbserver=<db server> -e dbuser=<db user> -e dbpassword=<db password> -e dbcatalog=<dbcatalog> <name-of-image>
```
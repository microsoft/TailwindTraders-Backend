# Run Backend Services Locally

The easiest way to run your backend services locally is using _Compose_. To run the services type `docker-compose up` from terminal located in `./Source` folder. This will build (if needed) the Docker images and bring up all the containers.

**Note:** Only Linux containers are supported currently.

## Configurate containers

By default compose file configures all containers to use a SQL Server container, so you don't need to provide any specific configuration. But **Shopping cart API requires additional configuration** that must be provided using environment variables, or even better, through an `.env` file.

To do so, just create a file named `.env` in the same `./Source` folder with following content:

```
COSMOSDB_HOST=<Url of your CosmosDb>
COSMOSDB_AUTHKEY=<AuthKey of your CosmosDb>
```

If you are using Windows, you can run the [CosmosDb emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator). If using it, you can use following `.env` file:

```
COSMOSDB_HOST=https://10.75.0.1:8081/
COSMOSDB_AUTHKEY=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==
```

## Running using Visual Studio

To run the Backend using Visual Studio, just open the `Tailwind.Traders.Backend.sln`, and set "Docker-compose" as startup project and run the solution. Visual Studio will use the compose file to build and run all the containers.




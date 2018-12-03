FROM openjdk:8-jre AS base
WORKDIR /app

FROM openjdk:8-jdk AS maven
WORKDIR /src
COPY . .
RUN chmod +x ./mvnw
RUN ./mvnw install

FROM maven as build
WORKDIR /src
RUN ./mvnw package

FROM base as final
WORKDIR /app
COPY --from=build /src/target/Tailwind.Traders.Stock.Api-0.0.1-SNAPSHOT.jar .
COPY --from=build /src/setup/StockProduct.csv ./setup/
EXPOSE 8080
ENTRYPOINT exec java -Djava.security.egd=file:/dev/./urandom -jar /app/Tailwind.Traders.Stock.Api-0.0.1-SNAPSHOT.jar

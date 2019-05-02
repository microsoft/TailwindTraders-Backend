FROM maven:3.5-jdk-8-slim
EXPOSE 8080

WORKDIR /usr/src/app
COPY pom.xml ./
RUN /usr/local/bin/mvn-entrypoint.sh \
    mvn package -Dmaven.test.skip=true -Dcheckstyle.skip=true -Dmaven.javadoc.skip=true --fail-never
COPY . .
RUN mvn package -Dmaven.test.skip=true -Dcheckstyle.skip=true -Dmaven.javadoc.skip=true

ENTRYPOINT ["java","-jar","target/Tailwind.Traders.Stock.Api-0.0.1-SNAPSHOT.jar"]
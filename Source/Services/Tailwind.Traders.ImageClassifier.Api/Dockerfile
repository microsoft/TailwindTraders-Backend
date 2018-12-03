ARG sdkTag=2.1-sdk
ARG runtimeTag=2.1-aspnetcore-runtime
ARG coreversion=netcore21
ARG image=microsoft/dotnet
FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80

# install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*

FROM ${image}:${sdkTag} AS build
ARG coreversion
WORKDIR /src
COPY ./Directory.Build.props .
COPY ./build ./build
WORKDIR /src/project
COPY ./Services/Tailwind.Traders.ImageClassifier.Api/Tailwind.Traders.ImageClassifier.Api.csproj .
RUN dotnet restore -p:NetCoreVersion=${coreversion} 
COPY ./Services/Tailwind.Traders.ImageClassifier.Api .
RUN dotnet build -p:NetCoreVersion=${coreversion} -c Release -o /app

FROM build AS publish
ARG coreversion
RUN dotnet publish "Tailwind.Traders.ImageClassifier.Api.csproj" -p:NetCoreVersion=${coreversion} -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/project/model model
ENTRYPOINT ["dotnet", "Tailwind.Traders.ImageClassifier.Api.dll"]

ARG sdkTag=5.0
ARG runtimeTag=5.0
ARG image=mcr.microsoft.com/dotnet/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/sdk

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

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src
COPY ./Directory.Build.props .
COPY ./build ./build
WORKDIR /src/project
COPY ./Services/Tailwind.Traders.ImageClassifier.Api/Tailwind.Traders.ImageClassifier.Api.csproj .
RUN dotnet restore 
COPY ./Services/Tailwind.Traders.ImageClassifier.Api .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish "Tailwind.Traders.ImageClassifier.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/project/model model
ENTRYPOINT ["dotnet", "Tailwind.Traders.ImageClassifier.Api.dll"]

ARG sdkTag=5.0
ARG runtimeTag=5.0
ARG image=mcr.microsoft.com/dotnet/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/sdk

FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src
COPY ./Directory.Build.props .
COPY ./build ./build
WORKDIR /src/project
COPY ./Services/Tailwind.Traders.Login.Api .
RUN dotnet build "Tailwind.Traders.Login.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Tailwind.Traders.Login.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Tailwind.Traders.Login.Api.dll"]
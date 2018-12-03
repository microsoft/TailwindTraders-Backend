ARG sdkTag=2.1-sdk
ARG runtimeTag=2.1-aspnetcore-runtime
ARG coreversion=netcore21
ARG image=microsoft/dotnet
FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${image}:${sdkTag} AS build
ARG coreversion
WORKDIR /src
COPY ./Directory.Build.props .
COPY ./build ./build
WORKDIR /src/project
COPY ./ApiGWs/Tailwind.Traders.WebBff .
RUN dotnet build "Tailwind.Traders.WebBff.csproj"  -p:NetCoreVersion=${coreversion} -c Release -o /app

FROM build AS publish
ARG coreversion
RUN dotnet publish "Tailwind.Traders.WebBff.csproj"  -p:NetCoreVersion=${coreversion} -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Tailwind.Traders.WebBff.dll"]
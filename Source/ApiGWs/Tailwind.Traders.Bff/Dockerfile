ARG sdkTag=2.2
ARG runtimeTag=2.2
ARG image=mcr.microsoft.com/dotnet/core/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/core/sdk
FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src
COPY ./Directory.Build.props .
COPY ./build ./build
WORKDIR /src/project
COPY ./ApiGWs/Tailwind.Traders.Bff .
RUN dotnet build "Tailwind.Traders.MobileBff.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Tailwind.Traders.MobileBff.csproj"  -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Tailwind.Traders.MobileBff.dll"]
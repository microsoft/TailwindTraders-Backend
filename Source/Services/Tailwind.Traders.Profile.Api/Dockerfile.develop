FROM microsoft/dotnet:2.2-sdk
ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
EXPOSE 80

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["build/", "./build"]
COPY ["Services/Tailwind.Traders.Profile.Api/Tailwind.Traders.Profile.Api.csproj", "Services/Tailwind.Traders.Profile.Api/"]
RUN dotnet restore Services/Tailwind.Traders.Profile.Api/Tailwind.Traders.Profile.Api.csproj -nowarn:msb3202,nu1503
COPY . .
WORKDIR "/src/Services/Tailwind.Traders.Profile.Api/"
RUN dotnet build  --no-restore -c $BUILD_CONFIGURATION
ENTRYPOINT ["dotnet", "run", "--no-build", "--no-launch-profile", "-c", "$BUILD_CONFIGURATION", "--"]

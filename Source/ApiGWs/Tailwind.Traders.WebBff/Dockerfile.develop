FROM microsoft/dotnet:2.2-sdk
ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
EXPOSE 80

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["build/", "./build"]
COPY ["ApiGWs/Tailwind.Traders.WebBff/Tailwind.Traders.WebBff.csproj", "ApiGWs/Tailwind.Traders.WebBff/"]
RUN dotnet restore ApiGWs/Tailwind.Traders.WebBff/Tailwind.Traders.WebBff.csproj -nowarn:msb3202,nu1503
COPY . .
WORKDIR "/src/ApiGWs/Tailwind.Traders.WebBff/"
RUN dotnet build  --no-restore -c $BUILD_CONFIGURATION
ENTRYPOINT ["dotnet", "run", "--no-build", "--no-launch-profile", "-c", "$BUILD_CONFIGURATION", "--"]

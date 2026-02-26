ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet:10.0-alpine
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG VERSION=0.0.0
WORKDIR /src

# Copy solution and build config files
COPY src/backend/BauDoku.slnx .
COPY src/backend/Directory.Build.props .
COPY src/backend/Directory.Packages.props .

# Copy project files for restore
COPY src/backend/Tools/BauDoku.ResourceService/BauDoku.ResourceService.csproj Tools/BauDoku.ResourceService/
COPY src/backend/BuildingBlocks/BauDoku.ServiceDefaults/BauDoku.ServiceDefaults.csproj BuildingBlocks/BauDoku.ServiceDefaults/
COPY src/backend/BuildingBlocks/BauDoku.BuildingBlocks.Domain/BauDoku.BuildingBlocks.Domain.csproj BuildingBlocks/BauDoku.BuildingBlocks.Domain/

RUN dotnet restore Tools/BauDoku.ResourceService/BauDoku.ResourceService.csproj

# Copy source code
COPY src/backend/ .

RUN dotnet publish Tools/BauDoku.ResourceService/BauDoku.ResourceService.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:Version=$VERSION \
    /p:UseAppHost=false

FROM $RUNTIME_IMAGE AS final
WORKDIR /app
EXPOSE 5001 5002
USER $APP_UID
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BauDoku.ResourceService.dll"]

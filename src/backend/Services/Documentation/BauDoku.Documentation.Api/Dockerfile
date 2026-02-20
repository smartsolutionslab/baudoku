FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG VERSION=0.0.0
WORKDIR /src

# Copy solution and build config files
COPY src/backend/BauDoku.slnx .
COPY src/backend/Directory.Build.props .
COPY src/backend/Directory.Packages.props .

# Copy project files for restore
COPY src/backend/BuildingBlocks/BauDoku.BuildingBlocks.Domain/BauDoku.BuildingBlocks.Domain.csproj BuildingBlocks/BauDoku.BuildingBlocks.Domain/
COPY src/backend/BuildingBlocks/BauDoku.BuildingBlocks.Application/BauDoku.BuildingBlocks.Application.csproj BuildingBlocks/BauDoku.BuildingBlocks.Application/
COPY src/backend/BuildingBlocks/BauDoku.BuildingBlocks.Infrastructure/BauDoku.BuildingBlocks.Infrastructure.csproj BuildingBlocks/BauDoku.BuildingBlocks.Infrastructure/
COPY src/backend/BuildingBlocks/BauDoku.ServiceDefaults/BauDoku.ServiceDefaults.csproj BuildingBlocks/BauDoku.ServiceDefaults/
COPY src/backend/Services/Documentation/BauDoku.Documentation.Domain/BauDoku.Documentation.Domain.csproj Services/Documentation/BauDoku.Documentation.Domain/
COPY src/backend/Services/Documentation/BauDoku.Documentation.Application/BauDoku.Documentation.Application.csproj Services/Documentation/BauDoku.Documentation.Application/
COPY src/backend/Services/Documentation/BauDoku.Documentation.Infrastructure/BauDoku.Documentation.Infrastructure.csproj Services/Documentation/BauDoku.Documentation.Infrastructure/
COPY src/backend/Services/Documentation/BauDoku.Documentation.Api/BauDoku.Documentation.Api.csproj Services/Documentation/BauDoku.Documentation.Api/

RUN dotnet restore Services/Documentation/BauDoku.Documentation.Api/BauDoku.Documentation.Api.csproj

# Copy source code
COPY src/backend/ .

RUN dotnet publish Services/Documentation/BauDoku.Documentation.Api/BauDoku.Documentation.Api.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:Version=$VERSION \
    /p:UseAppHost=false

ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet:10.0-preview-alpine
FROM $RUNTIME_IMAGE AS final
WORKDIR /app
EXPOSE 8080
USER $APP_UID
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BauDoku.Documentation.Api.dll"]

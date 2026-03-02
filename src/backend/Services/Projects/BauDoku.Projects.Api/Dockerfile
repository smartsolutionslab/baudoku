ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet:10.0-alpine
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
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
COPY src/backend/BuildingBlocks/BauDoku.BuildingBlocks.Guards/BauDoku.BuildingBlocks.Guards.csproj BuildingBlocks/BauDoku.BuildingBlocks.Guards/
COPY src/backend/BuildingBlocks/BauDoku.BuildingBlocks.Auth/BauDoku.BuildingBlocks.Auth.csproj BuildingBlocks/BauDoku.BuildingBlocks.Auth/
COPY src/backend/BuildingBlocks/BauDoku.BuildingBlocks.Persistence/BauDoku.BuildingBlocks.Persistence.csproj BuildingBlocks/BauDoku.BuildingBlocks.Persistence/
COPY src/backend/BuildingBlocks/BauDoku.BuildingBlocks.Serialization/BauDoku.BuildingBlocks.Serialization.csproj BuildingBlocks/BauDoku.BuildingBlocks.Serialization/
COPY src/backend/BuildingBlocks/BauDoku.ServiceDefaults/BauDoku.ServiceDefaults.csproj BuildingBlocks/BauDoku.ServiceDefaults/
COPY src/backend/Services/Projects/BauDoku.Projects.Domain/BauDoku.Projects.Domain.csproj Services/Projects/BauDoku.Projects.Domain/
COPY src/backend/Services/Projects/BauDoku.Projects.Application/BauDoku.Projects.Application.csproj Services/Projects/BauDoku.Projects.Application/
COPY src/backend/Services/Projects/BauDoku.Projects.Infrastructure/BauDoku.Projects.Infrastructure.csproj Services/Projects/BauDoku.Projects.Infrastructure/
COPY src/backend/Services/Projects/BauDoku.Projects.Api/BauDoku.Projects.Api.csproj Services/Projects/BauDoku.Projects.Api/

RUN dotnet restore Services/Projects/BauDoku.Projects.Api/BauDoku.Projects.Api.csproj

# Copy source code
COPY src/backend/ .

RUN dotnet publish Services/Projects/BauDoku.Projects.Api/BauDoku.Projects.Api.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:Version=$VERSION \
    /p:UseAppHost=false

FROM $RUNTIME_IMAGE AS final
WORKDIR /app
EXPOSE 8080
USER $APP_UID
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BauDoku.Projects.Api.dll"]

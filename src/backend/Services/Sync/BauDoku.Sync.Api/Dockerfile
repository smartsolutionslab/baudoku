FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
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
COPY src/backend/Services/Sync/BauDoku.Sync.Domain/BauDoku.Sync.Domain.csproj Services/Sync/BauDoku.Sync.Domain/
COPY src/backend/Services/Sync/BauDoku.Sync.Application/BauDoku.Sync.Application.csproj Services/Sync/BauDoku.Sync.Application/
COPY src/backend/Services/Sync/BauDoku.Sync.Infrastructure/BauDoku.Sync.Infrastructure.csproj Services/Sync/BauDoku.Sync.Infrastructure/
COPY src/backend/Services/Sync/BauDoku.Sync.Api/BauDoku.Sync.Api.csproj Services/Sync/BauDoku.Sync.Api/

RUN dotnet restore BauDoku.slnx --project Services/Sync/BauDoku.Sync.Api/BauDoku.Sync.Api.csproj

# Copy source code
COPY src/backend/ .

RUN dotnet publish Services/Sync/BauDoku.Sync.Api/BauDoku.Sync.Api.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:Version=$VERSION \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BauDoku.Sync.Api.dll"]

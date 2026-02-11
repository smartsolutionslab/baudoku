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
COPY src/backend/ApiGateway/BauDoku.ApiGateway/BauDoku.ApiGateway.csproj ApiGateway/BauDoku.ApiGateway/

RUN dotnet restore ApiGateway/BauDoku.ApiGateway/BauDoku.ApiGateway.csproj

# Copy source code
COPY src/backend/ .

RUN dotnet publish ApiGateway/BauDoku.ApiGateway/BauDoku.ApiGateway.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:Version=$VERSION \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BauDoku.ApiGateway.dll"]

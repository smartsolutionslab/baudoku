#!/bin/bash
# ============================================
# BauDoku – Phase 0 Foundation Setup Script
# Erstellt die komplette .NET Solution-Struktur
# ============================================

set -e
echo "🏗️  BauDoku – Erstelle .NET 10 Solution-Struktur..."

# Basis-Verzeichnis
mkdir -p src/backend
cd src/backend

# Solution erstellen
dotnet new sln -n BauDoku
echo "✅ Solution erstellt"

# ---- BuildingBlocks ----
echo "📦 BuildingBlocks..."
dotnet new classlib -n BauDoku.BuildingBlocks.Domain -o BuildingBlocks/BauDoku.BuildingBlocks.Domain -f net10.0
dotnet new classlib -n BauDoku.BuildingBlocks.Application -o BuildingBlocks/BauDoku.BuildingBlocks.Application -f net10.0
dotnet new classlib -n BauDoku.BuildingBlocks.Infrastructure -o BuildingBlocks/BauDoku.BuildingBlocks.Infrastructure -f net10.0

# BuildingBlocks References
dotnet add BuildingBlocks/BauDoku.BuildingBlocks.Application reference BuildingBlocks/BauDoku.BuildingBlocks.Domain
dotnet add BuildingBlocks/BauDoku.BuildingBlocks.Infrastructure reference BuildingBlocks/BauDoku.BuildingBlocks.Application

# ---- Projects BC ----
echo "📦 Projects Bounded Context..."
dotnet new classlib -n BauDoku.Projects.Domain -o Services/Projects/BauDoku.Projects.Domain -f net10.0
dotnet new classlib -n BauDoku.Projects.Application -o Services/Projects/BauDoku.Projects.Application -f net10.0
dotnet new classlib -n BauDoku.Projects.Infrastructure -o Services/Projects/BauDoku.Projects.Infrastructure -f net10.0
dotnet new webapi -n BauDoku.Projects.Api -o Services/Projects/BauDoku.Projects.Api -f net10.0 --no-openapi

dotnet add Services/Projects/BauDoku.Projects.Domain reference BuildingBlocks/BauDoku.BuildingBlocks.Domain
dotnet add Services/Projects/BauDoku.Projects.Application reference Services/Projects/BauDoku.Projects.Domain
dotnet add Services/Projects/BauDoku.Projects.Application reference BuildingBlocks/BauDoku.BuildingBlocks.Application
dotnet add Services/Projects/BauDoku.Projects.Infrastructure reference Services/Projects/BauDoku.Projects.Application
dotnet add Services/Projects/BauDoku.Projects.Infrastructure reference BuildingBlocks/BauDoku.BuildingBlocks.Infrastructure
dotnet add Services/Projects/BauDoku.Projects.Api reference Services/Projects/BauDoku.Projects.Infrastructure

# ---- Documentation BC ----
echo "📦 Documentation Bounded Context..."
dotnet new classlib -n BauDoku.Documentation.Domain -o Services/Documentation/BauDoku.Documentation.Domain -f net10.0
dotnet new classlib -n BauDoku.Documentation.Application -o Services/Documentation/BauDoku.Documentation.Application -f net10.0
dotnet new classlib -n BauDoku.Documentation.Infrastructure -o Services/Documentation/BauDoku.Documentation.Infrastructure -f net10.0
dotnet new webapi -n BauDoku.Documentation.Api -o Services/Documentation/BauDoku.Documentation.Api -f net10.0 --no-openapi

dotnet add Services/Documentation/BauDoku.Documentation.Domain reference BuildingBlocks/BauDoku.BuildingBlocks.Domain
dotnet add Services/Documentation/BauDoku.Documentation.Application reference Services/Documentation/BauDoku.Documentation.Domain
dotnet add Services/Documentation/BauDoku.Documentation.Application reference BuildingBlocks/BauDoku.BuildingBlocks.Application
dotnet add Services/Documentation/BauDoku.Documentation.Infrastructure reference Services/Documentation/BauDoku.Documentation.Application
dotnet add Services/Documentation/BauDoku.Documentation.Infrastructure reference BuildingBlocks/BauDoku.BuildingBlocks.Infrastructure
dotnet add Services/Documentation/BauDoku.Documentation.Api reference Services/Documentation/BauDoku.Documentation.Infrastructure

# ---- Sync BC ----
echo "📦 Sync Bounded Context..."
dotnet new classlib -n BauDoku.Sync.Domain -o Services/Sync/BauDoku.Sync.Domain -f net10.0
dotnet new classlib -n BauDoku.Sync.Application -o Services/Sync/BauDoku.Sync.Application -f net10.0
dotnet new classlib -n BauDoku.Sync.Infrastructure -o Services/Sync/BauDoku.Sync.Infrastructure -f net10.0
dotnet new webapi -n BauDoku.Sync.Api -o Services/Sync/BauDoku.Sync.Api -f net10.0 --no-openapi

dotnet add Services/Sync/BauDoku.Sync.Domain reference BuildingBlocks/BauDoku.BuildingBlocks.Domain
dotnet add Services/Sync/BauDoku.Sync.Application reference Services/Sync/BauDoku.Sync.Domain
dotnet add Services/Sync/BauDoku.Sync.Application reference BuildingBlocks/BauDoku.BuildingBlocks.Application
dotnet add Services/Sync/BauDoku.Sync.Infrastructure reference Services/Sync/BauDoku.Sync.Application
dotnet add Services/Sync/BauDoku.Sync.Infrastructure reference BuildingBlocks/BauDoku.BuildingBlocks.Infrastructure
dotnet add Services/Sync/BauDoku.Sync.Api reference Services/Sync/BauDoku.Sync.Infrastructure

# ---- API Gateway ----
echo "📦 API Gateway..."
dotnet new webapi -n BauDoku.ApiGateway -o ApiGateway/BauDoku.ApiGateway -f net10.0 --no-openapi

# ---- Aspire AppHost ----
echo "📦 .NET Aspire AppHost..."
# AppHost wird manuell erstellt (Aspire Templates müssen installiert sein)
mkdir -p AppHost/BauDoku.AppHost

# ---- Tests ----
echo "🧪 Test-Projekte..."
dotnet new xunit -n BauDoku.Projects.UnitTests -o ../../tests/backend/BauDoku.Projects.UnitTests -f net10.0
dotnet new xunit -n BauDoku.Documentation.UnitTests -o ../../tests/backend/BauDoku.Documentation.UnitTests -f net10.0
dotnet new xunit -n BauDoku.Documentation.IntegrationTests -o ../../tests/backend/BauDoku.Documentation.IntegrationTests -f net10.0
dotnet new xunit -n BauDoku.Sync.IntegrationTests -o ../../tests/backend/BauDoku.Sync.IntegrationTests -f net10.0

dotnet add ../../tests/backend/BauDoku.Projects.UnitTests reference Services/Projects/BauDoku.Projects.Domain
dotnet add ../../tests/backend/BauDoku.Projects.UnitTests reference Services/Projects/BauDoku.Projects.Application
dotnet add ../../tests/backend/BauDoku.Documentation.UnitTests reference Services/Documentation/BauDoku.Documentation.Domain
dotnet add ../../tests/backend/BauDoku.Documentation.UnitTests reference Services/Documentation/BauDoku.Documentation.Application

# ---- Alle Projekte zur Solution hinzufügen ----
echo "📎 Projekte zur Solution hinzufügen..."

# BuildingBlocks
dotnet sln add BuildingBlocks/BauDoku.BuildingBlocks.Domain --solution-folder BuildingBlocks
dotnet sln add BuildingBlocks/BauDoku.BuildingBlocks.Application --solution-folder BuildingBlocks
dotnet sln add BuildingBlocks/BauDoku.BuildingBlocks.Infrastructure --solution-folder BuildingBlocks

# Projects BC
dotnet sln add Services/Projects/BauDoku.Projects.Domain --solution-folder Services/Projects
dotnet sln add Services/Projects/BauDoku.Projects.Application --solution-folder Services/Projects
dotnet sln add Services/Projects/BauDoku.Projects.Infrastructure --solution-folder Services/Projects
dotnet sln add Services/Projects/BauDoku.Projects.Api --solution-folder Services/Projects

# Documentation BC
dotnet sln add Services/Documentation/BauDoku.Documentation.Domain --solution-folder Services/Documentation
dotnet sln add Services/Documentation/BauDoku.Documentation.Application --solution-folder Services/Documentation
dotnet sln add Services/Documentation/BauDoku.Documentation.Infrastructure --solution-folder Services/Documentation
dotnet sln add Services/Documentation/BauDoku.Documentation.Api --solution-folder Services/Documentation

# Sync BC
dotnet sln add Services/Sync/BauDoku.Sync.Domain --solution-folder Services/Sync
dotnet sln add Services/Sync/BauDoku.Sync.Application --solution-folder Services/Sync
dotnet sln add Services/Sync/BauDoku.Sync.Infrastructure --solution-folder Services/Sync
dotnet sln add Services/Sync/BauDoku.Sync.Api --solution-folder Services/Sync

# Gateway
dotnet sln add ApiGateway/BauDoku.ApiGateway --solution-folder Gateway

# Tests
dotnet sln add ../../tests/backend/BauDoku.Projects.UnitTests --solution-folder Tests
dotnet sln add ../../tests/backend/BauDoku.Documentation.UnitTests --solution-folder Tests
dotnet sln add ../../tests/backend/BauDoku.Documentation.IntegrationTests --solution-folder Tests
dotnet sln add ../../tests/backend/BauDoku.Sync.IntegrationTests --solution-folder Tests

# ---- Cleanup: Dummy-Dateien löschen ----
echo "🧹 Cleanup..."
find . -name "Class1.cs" -delete
find . -name "WeatherForecast*.cs" -delete 2>/dev/null || true

echo ""
echo "✅ BauDoku Solution erstellt!"
echo "   18 Projekte, 3 Bounded Contexts, BuildingBlocks, Gateway, Tests"
echo ""
echo "Nächster Schritt: dotnet build BauDoku.sln"

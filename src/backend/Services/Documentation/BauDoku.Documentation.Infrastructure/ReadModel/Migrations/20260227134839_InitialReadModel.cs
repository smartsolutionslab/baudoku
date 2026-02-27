using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Documentation.Infrastructure.ReadModel.Migrations
{
    /// <inheritdoc />
    public partial class InitialReadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "documentation_read");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "installations",
                schema: "documentation_read",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Altitude = table.Column<double>(type: "double precision", nullable: true),
                    HorizontalAccuracy = table.Column<double>(type: "double precision", nullable: false),
                    GpsSource = table.Column<string>(type: "text", nullable: false),
                    CorrectionService = table.Column<string>(type: "text", nullable: true),
                    RtkFixStatus = table.Column<string>(type: "text", nullable: true),
                    SatelliteCount = table.Column<int>(type: "integer", nullable: true),
                    Hdop = table.Column<double>(type: "double precision", nullable: true),
                    CorrectionAge = table.Column<double>(type: "double precision", nullable: true),
                    QualityGrade = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CableType = table.Column<string>(type: "text", nullable: true),
                    CrossSection = table.Column<decimal>(type: "numeric", nullable: true),
                    CableColor = table.Column<string>(type: "text", nullable: true),
                    ConductorCount = table.Column<int>(type: "integer", nullable: true),
                    DepthMm = table.Column<int>(type: "integer", nullable: true),
                    Manufacturer = table.Column<string>(type: "text", nullable: true),
                    ModelName = table.Column<string>(type: "text", nullable: true),
                    SerialNumber = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PhotoCount = table.Column<int>(type: "integer", nullable: false),
                    MeasurementCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_installations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "measurements",
                schema: "documentation_read",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstallationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    MinThreshold = table.Column<double>(type: "double precision", nullable: true),
                    MaxThreshold = table.Column<double>(type: "double precision", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    MeasuredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_measurements_installations_InstallationId",
                        column: x => x.InstallationId,
                        principalSchema: "documentation_read",
                        principalTable: "installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "photos",
                schema: "documentation_read",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstallationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    BlobUrl = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    PhotoType = table.Column<string>(type: "text", nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Altitude = table.Column<double>(type: "double precision", nullable: true),
                    HorizontalAccuracy = table.Column<double>(type: "double precision", nullable: true),
                    GpsSource = table.Column<string>(type: "text", nullable: true),
                    CorrectionService = table.Column<string>(type: "text", nullable: true),
                    RtkFixStatus = table.Column<string>(type: "text", nullable: true),
                    SatelliteCount = table.Column<int>(type: "integer", nullable: true),
                    Hdop = table.Column<double>(type: "double precision", nullable: true),
                    CorrectionAge = table.Column<double>(type: "double precision", nullable: true),
                    TakenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_photos_installations_InstallationId",
                        column: x => x.InstallationId,
                        principalSchema: "documentation_read",
                        principalTable: "installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_installations_IsDeleted",
                schema: "documentation_read",
                table: "installations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_installations_ProjectId",
                schema: "documentation_read",
                table: "installations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_installations_Status",
                schema: "documentation_read",
                table: "installations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_installations_ZoneId",
                schema: "documentation_read",
                table: "installations",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_measurements_InstallationId",
                schema: "documentation_read",
                table: "measurements",
                column: "InstallationId");

            migrationBuilder.CreateIndex(
                name: "IX_photos_InstallationId",
                schema: "documentation_read",
                table: "photos",
                column: "InstallationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "measurements",
                schema: "documentation_read");

            migrationBuilder.DropTable(
                name: "photos",
                schema: "documentation_read");

            migrationBuilder.DropTable(
                name: "installations",
                schema: "documentation_read");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Documentation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "installations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    gps_latitude = table.Column<double>(type: "double precision", nullable: false),
                    gps_longitude = table.Column<double>(type: "double precision", nullable: false),
                    gps_altitude = table.Column<double>(type: "double precision", nullable: true),
                    gps_horizontal_accuracy = table.Column<double>(type: "double precision", nullable: false),
                    gps_source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    gps_correction_service = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    gps_rtk_fix_status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    gps_satellite_count = table.Column<int>(type: "integer", nullable: true),
                    gps_hdop = table.Column<double>(type: "double precision", nullable: true),
                    gps_correction_age = table.Column<double>(type: "double precision", nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    cable_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cable_cross_section = table.Column<int>(type: "integer", nullable: true),
                    cable_color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cable_conductor_count = table.Column<int>(type: "integer", nullable: true),
                    depth_mm = table.Column<int>(type: "integer", nullable: true),
                    manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    model_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    serial_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_installations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "measurements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    measurement_value = table.Column<double>(type: "double precision", nullable: false),
                    measurement_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    installation_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurements", x => x.id);
                    table.ForeignKey(
                        name: "FK_measurements_installations_installation_id",
                        column: x => x.installation_id,
                        principalTable: "installations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "photos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    captured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gps_latitude = table.Column<double>(type: "double precision", nullable: true),
                    gps_longitude = table.Column<double>(type: "double precision", nullable: true),
                    gps_altitude = table.Column<double>(type: "double precision", nullable: true),
                    gps_horizontal_accuracy = table.Column<double>(type: "double precision", nullable: true),
                    gps_source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    gps_correction_service = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    gps_rtk_fix_status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    gps_satellite_count = table.Column<int>(type: "integer", nullable: true),
                    gps_hdop = table.Column<double>(type: "double precision", nullable: true),
                    gps_correction_age = table.Column<double>(type: "double precision", nullable: true),
                    installation_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_photos_installations_installation_id",
                        column: x => x.installation_id,
                        principalTable: "installations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                ALTER TABLE installations ADD COLUMN location geometry(Point, 4326)
                GENERATED ALWAYS AS (ST_SetSRID(ST_MakePoint(gps_longitude, gps_latitude), 4326)) STORED;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_measurements_installation_id",
                table: "measurements",
                column: "installation_id");

            migrationBuilder.CreateIndex(
                name: "IX_photos_installation_id",
                table: "photos",
                column: "installation_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "measurements");

            migrationBuilder.DropTable(
                name: "photos");

            migrationBuilder.DropTable(
                name: "installations");
        }
    }
}

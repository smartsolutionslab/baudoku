using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Documentation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndDecimalCrossSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "cable_cross_section",
                table: "installations",
                type: "numeric(5,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gps_quality_grade",
                table: "installations",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_installations_project_id",
                table: "installations",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_installations_status",
                table: "installations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_installations_zone_id",
                table: "installations",
                column: "zone_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_installations_project_id",
                table: "installations");

            migrationBuilder.DropIndex(
                name: "ix_installations_status",
                table: "installations");

            migrationBuilder.DropIndex(
                name: "ix_installations_zone_id",
                table: "installations");

            migrationBuilder.DropColumn(
                name: "gps_quality_grade",
                table: "installations");

            migrationBuilder.AlterColumn<int>(
                name: "cable_cross_section",
                table: "installations",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldNullable: true);
        }
    }
}

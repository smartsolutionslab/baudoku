using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Documentation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMeasurementResultAndThresholds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "result",
                table: "measurements",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "passed");

            migrationBuilder.AddColumn<double>(
                name: "min_threshold",
                table: "measurements",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "max_threshold",
                table: "measurements",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "result",
                table: "measurements");

            migrationBuilder.DropColumn(
                name: "min_threshold",
                table: "measurements");

            migrationBuilder.DropColumn(
                name: "max_threshold",
                table: "measurements");
        }
    }
}

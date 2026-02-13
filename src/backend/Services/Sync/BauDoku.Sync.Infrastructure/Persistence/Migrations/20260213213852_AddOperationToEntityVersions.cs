using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Sync.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationToEntityVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "operation",
                table: "entity_versions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "update");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "entity_versions",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "operation",
                table: "entity_versions");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "entity_versions");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Projects.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectAndZoneIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_zones_project_id",
                table: "zones",
                newName: "ix_zones_project_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_name",
                table: "projects",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_projects_status",
                table: "projects",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_projects_name",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "ix_projects_status",
                table: "projects");

            migrationBuilder.RenameIndex(
                name: "ix_zones_project_id",
                table: "zones",
                newName: "IX_zones_project_id");
        }
    }
}

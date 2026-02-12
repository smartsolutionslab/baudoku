using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Documentation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSpatialIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The location column already exists (created via raw SQL in InitialCreate).
            // This migration adds the shadow property to the EF model snapshot
            // and creates the GiST spatial index for efficient spatial queries.
            migrationBuilder.CreateIndex(
                name: "IX_installations_location",
                table: "installations",
                column: "location")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_installations_location",
                table: "installations");
        }
    }
}

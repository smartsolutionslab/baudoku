using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Documentation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_path",
                table: "photos");

            migrationBuilder.RenameColumn(
                name: "captured_at",
                table: "photos",
                newName: "taken_at");

            migrationBuilder.AddColumn<string>(
                name: "file_name",
                table: "photos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "blob_url",
                table: "photos",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "content_type",
                table: "photos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "file_size",
                table: "photos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "photo_type",
                table: "photos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "caption",
                table: "photos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_name",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "blob_url",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "content_type",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "file_size",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "photo_type",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "caption",
                table: "photos");

            migrationBuilder.RenameColumn(
                name: "taken_at",
                table: "photos",
                newName: "captured_at");

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "photos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}

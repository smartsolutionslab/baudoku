using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BauDoku.Sync.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entity_versions",
                columns: table => new
                {
                    entity_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_device_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_versions", x => new { x.entity_type, x.entity_id });
                });

            migrationBuilder.CreateTable(
                name: "sync_batches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_batches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sync_conflicts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_payload = table.Column<string>(type: "text", nullable: false),
                    server_payload = table.Column<string>(type: "text", nullable: false),
                    client_version = table.Column<long>(type: "bigint", nullable: false),
                    server_version = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    resolved_payload = table.Column<string>(type: "text", nullable: true),
                    detected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sync_batch_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_conflicts", x => x.id);
                    table.ForeignKey(
                        name: "FK_sync_conflicts_sync_batches_sync_batch_id",
                        column: x => x.sync_batch_id,
                        principalTable: "sync_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sync_deltas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    base_version = table.Column<long>(type: "bigint", nullable: false),
                    server_version = table.Column<long>(type: "bigint", nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sync_batch_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_deltas", x => x.id);
                    table.ForeignKey(
                        name: "FK_sync_deltas_sync_batches_sync_batch_id",
                        column: x => x.sync_batch_id,
                        principalTable: "sync_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_entity_versions_last_modified",
                table: "entity_versions",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "IX_sync_conflicts_sync_batch_id",
                table: "sync_conflicts",
                column: "sync_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_sync_deltas_sync_batch_id",
                table: "sync_deltas",
                column: "sync_batch_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity_versions");

            migrationBuilder.DropTable(
                name: "sync_conflicts");

            migrationBuilder.DropTable(
                name: "sync_deltas");

            migrationBuilder.DropTable(
                name: "sync_batches");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWallMessageReactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReactionCount",
                table: "WallMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WallMessageReactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    WallMessageId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WallMessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WallMessageReactions_WallMessages_WallMessageId",
                        column: x => x.WallMessageId,
                        principalTable: "WallMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WallMessageReactions_WallMessageId_DeviceId",
                table: "WallMessageReactions",
                columns: new[] { "WallMessageId", "DeviceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WallMessageReactions");

            migrationBuilder.DropColumn(
                name: "ReactionCount",
                table: "WallMessages");
        }
    }
}

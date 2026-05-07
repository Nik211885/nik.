using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Data
{
    /// <inheritdoc />
    public partial class UpdateSomething : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Albums_AlbumId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "Comments",
                type: "character varying(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)");

            migrationBuilder.AlterColumn<string>(
                name: "AlbumId",
                table: "Albums",
                type: "character varying(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Albums_AlbumId",
                table: "Albums",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments",
                column: "ParentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Albums_AlbumId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "Comments",
                type: "character varying(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlbumId",
                table: "Albums",
                type: "character varying(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Albums_AlbumId",
                table: "Albums",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments",
                column: "ParentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

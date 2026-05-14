using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Data
{
    /// <inheritdoc />
    public partial class MakeAlbumFileDescriptionIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Files_FileDescriptionId",
                table: "Albums");

            migrationBuilder.AlterColumn<string>(
                name: "FileDescriptionId",
                table: "Albums",
                type: "character varying(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Files_FileDescriptionId",
                table: "Albums",
                column: "FileDescriptionId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Files_FileDescriptionId",
                table: "Albums");

            migrationBuilder.AlterColumn<string>(
                name: "FileDescriptionId",
                table: "Albums",
                type: "character varying(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Files_FileDescriptionId",
                table: "Albums",
                column: "FileDescriptionId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

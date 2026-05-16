using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Data
{
    /// <inheritdoc />
    public partial class AddContentTranslationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentTranslations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Field = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LangCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTranslations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentTranslations_EntityType_EntityId_Field_LangCode",
                table: "ContentTranslations",
                columns: new[] { "EntityType", "EntityId", "Field", "LangCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTranslations_EntityType_EntityId_LangCode",
                table: "ContentTranslations",
                columns: new[] { "EntityType", "EntityId", "LangCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentTranslations_EntityType_LangCode",
                table: "ContentTranslations",
                columns: new[] { "EntityType", "LangCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentTranslations");
        }
    }
}

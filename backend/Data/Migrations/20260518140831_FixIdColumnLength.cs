using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixIdColumnLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Increase all UUID-storing columns from varchar(20) to varchar(36)
            var alterStatements = new[]
            {
                "ALTER TABLE \"Albums\"              ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Albums\"              ALTER COLUMN \"AlbumId\"          TYPE character varying(36)",
                "ALTER TABLE \"Albums\"              ALTER COLUMN \"FileDescriptionId\" TYPE character varying(36)",
                "ALTER TABLE \"AlbumFiles\"          ALTER COLUMN \"AlbumId\"          TYPE character varying(36)",
                "ALTER TABLE \"AlbumFiles\"          ALTER COLUMN \"FileId\"           TYPE character varying(36)",
                "ALTER TABLE \"Articles\"            ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Articles\"            ALTER COLUMN \"AuthorId\"         TYPE character varying(36)",
                "ALTER TABLE \"ArticleCategories\"   ALTER COLUMN \"ArticleId\"        TYPE character varying(36)",
                "ALTER TABLE \"ArticleCategories\"   ALTER COLUMN \"CategoryId\"       TYPE character varying(36)",
                "ALTER TABLE \"ArticleTags\"         ALTER COLUMN \"ArticleId\"        TYPE character varying(36)",
                "ALTER TABLE \"ArticleTags\"         ALTER COLUMN \"TagId\"            TYPE character varying(36)",
                "ALTER TABLE \"Categories\"          ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"CodeLanguages\"       ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Comments\"            ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Comments\"            ALTER COLUMN \"ArticleId\"        TYPE character varying(36)",
                "ALTER TABLE \"Comments\"            ALTER COLUMN \"UserId\"           TYPE character varying(36)",
                "ALTER TABLE \"Comments\"            ALTER COLUMN \"ParentId\"         TYPE character varying(36)",
                "ALTER TABLE \"ContentTranslations\" ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Files\"               ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"HeroSlides\"          ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"HeroSlides\"          ALTER COLUMN \"FileId\"           TYPE character varying(36)",
                "ALTER TABLE \"Languages\"           ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"PageViews\"           ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Reactions\"           ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Reactions\"           ALTER COLUMN \"ArticleId\"        TYPE character varying(36)",
                "ALTER TABLE \"Reactions\"           ALTER COLUMN \"CreatedByUserId\"  TYPE character varying(36)",
                "ALTER TABLE \"SysConfigs\"          ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Tags\"                ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Translates\"          ALTER COLUMN \"Id\"               TYPE character varying(36)",
                "ALTER TABLE \"Translates\"          ALTER COLUMN \"CodeId\"           TYPE character varying(36)",
                "ALTER TABLE \"Translates\"          ALTER COLUMN \"LanguageId\"       TYPE character varying(36)",
                "ALTER TABLE \"Users\"               ALTER COLUMN \"Id\"               TYPE character varying(36)",
            };

            foreach (var sql in alterStatements)
                migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty — downgrading would risk truncating existing UUID data
        }
    }
}

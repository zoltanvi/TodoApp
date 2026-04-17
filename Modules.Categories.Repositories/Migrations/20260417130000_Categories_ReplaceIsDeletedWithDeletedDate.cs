using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Categories.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Categories_ReplaceIsDeletedWithDeletedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedDate",
                table: "Categories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE Categories SET DeletedDate = strftime('%Y-%m-%d__%H:%M:%S__000000', 'now', 'localtime') WHERE IsDeleted = 1");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                "UPDATE Categories SET IsDeleted = 1 WHERE DeletedDate IS NOT NULL");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Categories");
        }
    }
}

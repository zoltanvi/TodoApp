using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Tasks.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Tasks_RemoveIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                "UPDATE Tasks SET IsDeleted = 1 WHERE DeletedDate IS NOT NULL");
        }
    }
}

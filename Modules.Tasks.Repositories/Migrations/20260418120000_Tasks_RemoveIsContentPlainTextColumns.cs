using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Tasks.Repositories.Migrations;

/// <inheritdoc />
public partial class Tasks_RemoveIsContentPlainTextColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsContentPlainText",
            table: "Tasks");

        migrationBuilder.DropColumn(
            name: "IsContentPlainText",
            table: "TaskItemVersions");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsContentPlainText",
            table: "Tasks",
            type: "INTEGER",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsContentPlainText",
            table: "TaskItemVersions",
            type: "INTEGER",
            nullable: false,
            defaultValue: true);
    }
}

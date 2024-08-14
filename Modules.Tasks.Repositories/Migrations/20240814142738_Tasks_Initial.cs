using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Tasks.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Tasks_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    IsContentPlainText = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContentPreview = table.Column<string>(type: "TEXT", nullable: false),
                    ListOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Pinned = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDone = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<string>(type: "TEXT", nullable: false),
                    ModificationDate = table.Column<string>(type: "TEXT", nullable: false),
                    MarkerColor = table.Column<string>(type: "TEXT", nullable: false),
                    BorderColor = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedDate = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TasksDbInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Initialized = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasksDbInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReminderDate = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaskId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminders_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagItemTaskItem",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskItemsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagItemTaskItem", x => new { x.TagsId, x.TaskItemsId });
                    table.ForeignKey(
                        name: "FK_TagItemTaskItem_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagItemTaskItem_Tasks_TaskItemsId",
                        column: x => x.TaskItemsId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskItemVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    IsContentPlainText = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContentPreview = table.Column<string>(type: "TEXT", nullable: false),
                    VersionDate = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItemVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItemVersions_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_TaskId",
                table: "Reminders",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TagItemTaskItem_TaskItemsId",
                table: "TagItemTaskItem",
                column: "TaskItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItemVersions_TaskId",
                table: "TaskItemVersions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CategoryId",
                table: "Tasks",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "TagItemTaskItem");

            migrationBuilder.DropTable(
                name: "TaskItemVersions");

            migrationBuilder.DropTable(
                name: "TasksDbInfo");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}

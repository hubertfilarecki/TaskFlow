using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskPriorityAndAssignee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Assignee",
                table: "Tasks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assignee",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tasks");
        }
    }
}

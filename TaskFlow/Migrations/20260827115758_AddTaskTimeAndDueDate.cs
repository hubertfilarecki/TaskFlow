using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskTimeAndDueDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeSpentMinutes",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TimeSpentMinutes",
                table: "Tasks");
        }
    }
}

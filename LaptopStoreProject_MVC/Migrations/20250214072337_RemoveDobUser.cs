using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaptopStoreProject_MVC.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDobUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dob",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Dob",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }
    }
}

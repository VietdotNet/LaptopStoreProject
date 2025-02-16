using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaptopStoreProject_MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddDateOnlyUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Dob",
                table: "Users",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dob",
                table: "Users");
        }
    }
}

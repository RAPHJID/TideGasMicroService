using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CylinderService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Cylinders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedByStaffId",
                table: "Cylinders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoldToday",
                table: "Cylinders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalStock",
                table: "Cylinders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Cylinders");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByStaffId",
                table: "Cylinders");

            migrationBuilder.DropColumn(
                name: "SoldToday",
                table: "Cylinders");

            migrationBuilder.DropColumn(
                name: "TotalStock",
                table: "Cylinders");
        }
    }
}

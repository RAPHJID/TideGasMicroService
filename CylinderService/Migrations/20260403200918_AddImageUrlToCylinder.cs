using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CylinderService.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToCylinder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Cylinders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Cylinders");
        }
    }
}

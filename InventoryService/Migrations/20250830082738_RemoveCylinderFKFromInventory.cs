using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCylinderFKFromInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventorys_Cylinder_CylinderId",
                table: "Inventorys");

            migrationBuilder.DropTable(
                name: "Cylinder");

            migrationBuilder.DropIndex(
                name: "IX_Inventorys_CylinderId",
                table: "Inventorys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cylinder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastRefilled = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cylinder", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventorys_CylinderId",
                table: "Inventorys",
                column: "CylinderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventorys_Cylinder_CylinderId",
                table: "Inventorys",
                column: "CylinderId",
                principalTable: "Cylinder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

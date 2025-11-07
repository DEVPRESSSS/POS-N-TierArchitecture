using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointOfSale.Data.Migrations
{
    public partial class AddAllStockColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "allstock",
                table: "Product",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "allstock",
                table: "Product");
        }
    }
}

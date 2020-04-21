using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalStockTrader.Data.Migrations
{
    public partial class AddOpenPricePosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ClosePrice",
                table: "Positions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenPrice",
                table: "Positions",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosePrice",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "OpenPrice",
                table: "Positions");
        }
    }
}

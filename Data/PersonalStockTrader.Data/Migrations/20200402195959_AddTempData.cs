using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalStockTrader.Data.Migrations
{
    public partial class AddTempData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempDatas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastDateTime = table.Column<DateTime>(nullable: false),
                    LastPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempDatas", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempDatas");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalStockTrader.Data.Migrations
{
    public partial class UpdateContactFormTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "ContactFormEntries");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "ContactFormEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "ContactFormEntries");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ContactFormEntries",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

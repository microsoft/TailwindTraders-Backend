using Microsoft.EntityFrameworkCore.Migrations;

namespace Tailwind.Traders.Product.Api.Migrations
{
    public partial class addcodepropproducttypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ProductTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_Code",
                table: "ProductTypes",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_Code",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ProductTypes");
        }
    }
}

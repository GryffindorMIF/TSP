using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class renameproductproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Property",
                "ProductDetails");

            migrationBuilder.AddColumn<string>(
                "Name",
                "ProductDetails",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Name",
                "ProductDetails");

            migrationBuilder.AddColumn<string>(
                "Property",
                "ProductDetails",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class ProductDiscountRegularPriceRemoval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "RegularPrice",
                "ProductDiscount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                "RegularPrice",
                "ProductDiscount",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
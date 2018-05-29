using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class ProductDiscountRegularPriceAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Starts",
                "ProductDiscount");

            migrationBuilder.AddColumn<decimal>(
                "RegularPrice",
                "ProductDiscount",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "RegularPrice",
                "ProductDiscount");

            migrationBuilder.AddColumn<DateTime>(
                "Starts",
                "ProductDiscount",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
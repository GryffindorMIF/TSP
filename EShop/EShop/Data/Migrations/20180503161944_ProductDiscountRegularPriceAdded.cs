using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class ProductDiscountRegularPriceAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Starts",
                table: "ProductDiscount");

            migrationBuilder.AddColumn<decimal>(
                name: "RegularPrice",
                table: "ProductDiscount",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegularPrice",
                table: "ProductDiscount");

            migrationBuilder.AddColumn<DateTime>(
                name: "Starts",
                table: "ProductDiscount",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

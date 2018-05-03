using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class renameproductproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Property",
                table: "ProductDetails");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductDetails",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductDetails");

            migrationBuilder.AddColumn<string>(
                name: "Property",
                table: "ProductDetails",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}

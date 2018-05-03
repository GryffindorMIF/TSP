using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class OrderAddressBugfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryAddress_AddressId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_AddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Order",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Order",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Order_AddressId",
                table: "Order",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryAddress_AddressId",
                table: "Order",
                column: "AddressId",
                principalTable: "DeliveryAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

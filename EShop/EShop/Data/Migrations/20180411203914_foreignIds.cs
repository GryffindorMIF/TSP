using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class foreignIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProduct_Product_ProductId",
                table: "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                table: "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_ShoppingCart_ShoppingCartId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "ShoppingCartId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShoppingCartId",
                table: "ShoppingCartProduct",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ShoppingCartProduct",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProduct_Product_ProductId",
                table: "ShoppingCartProduct",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                table: "ShoppingCartProduct",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ShoppingCart_ShoppingCartId",
                table: "Users",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProduct_Product_ProductId",
                table: "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                table: "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_ShoppingCart_ShoppingCartId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "ShoppingCartId",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ShoppingCartId",
                table: "ShoppingCartProduct",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ShoppingCartProduct",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProduct_Product_ProductId",
                table: "ShoppingCartProduct",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                table: "ShoppingCartProduct",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ShoppingCart_ShoppingCartId",
                table: "Users",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

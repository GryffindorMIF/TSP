using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class foreignIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_ShoppingCartProduct_Product_ProductId",
                "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                "FK_Users_ShoppingCart_ShoppingCartId",
                "Users");

            migrationBuilder.AlterColumn<int>(
                "ShoppingCartId",
                "Users",
                nullable: true, //Before: false
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                "ShoppingCartId",
                "ShoppingCartProduct",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                "ProductId",
                "ShoppingCartProduct",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                "FK_ShoppingCartProduct_Product_ProductId",
                "ShoppingCartProduct",
                "ProductId",
                "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                "ShoppingCartProduct",
                "ShoppingCartId",
                "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_Users_ShoppingCart_ShoppingCartId",
                "Users",
                "ShoppingCartId",
                "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_ShoppingCartProduct_Product_ProductId",
                "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                "FK_Users_ShoppingCart_ShoppingCartId",
                "Users");

            migrationBuilder.AlterColumn<int>(
                "ShoppingCartId",
                "Users",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                "ShoppingCartId",
                "ShoppingCartProduct",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                "ProductId",
                "ShoppingCartProduct",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                "FK_ShoppingCartProduct_Product_ProductId",
                "ShoppingCartProduct",
                "ProductId",
                "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                "ShoppingCartProduct",
                "ShoppingCartId",
                "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_Users_ShoppingCart_ShoppingCartId",
                "Users",
                "ShoppingCartId",
                "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
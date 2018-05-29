using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class ShoppingCartAndProductStuffCascading : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_ProductImage_Product_ProductId",
                "ProductImage");

            migrationBuilder.DropForeignKey(
                "FK_ShoppingCartProduct_Product_ProductId",
                "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                "ShoppingCartProduct");

            migrationBuilder.AlterColumn<int>(
                "ShoppingCartId",
                "Users",
                nullable: false,
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

            migrationBuilder.AlterColumn<int>(
                "ProductId",
                "ProductImage",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                "FK_ProductImage_Product_ProductId",
                "ProductImage",
                "ProductId",
                "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_ProductImage_Product_ProductId",
                "ProductImage");

            migrationBuilder.DropForeignKey(
                "FK_ShoppingCartProduct_Product_ProductId",
                "ShoppingCartProduct");

            migrationBuilder.DropForeignKey(
                "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                "ShoppingCartProduct");

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

            migrationBuilder.AlterColumn<int>(
                "ProductId",
                "ProductImage",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                "FK_ProductImage_Product_ProductId",
                "ProductImage",
                "ProductId",
                "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
        }
    }
}
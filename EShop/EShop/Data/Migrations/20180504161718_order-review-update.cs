using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class orderreviewupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Order_ShoppingCart_ShoppingCartId",
                "Order");

            migrationBuilder.DropForeignKey(
                "FK_OrderReview_Order_OrderId",
                "OrderReview");

            migrationBuilder.DropIndex(
                "IX_OrderReview_OrderId",
                "OrderReview");

            migrationBuilder.DropIndex(
                "IX_Order_ShoppingCartId",
                "Order");

            migrationBuilder.AddColumn<int>(
                "Rating",
                "OrderReview",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Rating",
                "OrderReview");

            migrationBuilder.CreateIndex(
                "IX_OrderReview_OrderId",
                "OrderReview",
                "OrderId");

            migrationBuilder.CreateIndex(
                "IX_Order_ShoppingCartId",
                "Order",
                "ShoppingCartId");

            migrationBuilder.AddForeignKey(
                "FK_Order_ShoppingCart_ShoppingCartId",
                "Order",
                "ShoppingCartId",
                "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_OrderReview_Order_OrderId",
                "OrderReview",
                "OrderId",
                "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
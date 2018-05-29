using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class someStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Users_ShoppingCart_ShoppingCartId",
                "Users");

            migrationBuilder.DropIndex(
                "IX_Users_ShoppingCartId",
                "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                "IX_Users_ShoppingCartId",
                "Users",
                "ShoppingCartId");

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
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class UserUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "ShoppingCartId",
                "Users",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Users_ShoppingCart_ShoppingCartId",
                "Users");

            migrationBuilder.DropIndex(
                "IX_Users_ShoppingCartId",
                "Users");

            migrationBuilder.DropColumn(
                "ShoppingCartId",
                "Users");
        }
    }
}
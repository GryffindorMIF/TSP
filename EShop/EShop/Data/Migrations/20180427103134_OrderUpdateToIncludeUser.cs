using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class OrderUpdateToIncludeUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "UserId",
                "Order",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                "IX_Order_UserId",
                "Order",
                "UserId");

            migrationBuilder.AddForeignKey(
                "FK_Order_Users_UserId",
                "Order",
                "UserId",
                "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Order_Users_UserId",
                "Order");

            migrationBuilder.DropIndex(
                "IX_Order_UserId",
                "Order");

            migrationBuilder.DropColumn(
                "UserId",
                "Order");
        }
    }
}